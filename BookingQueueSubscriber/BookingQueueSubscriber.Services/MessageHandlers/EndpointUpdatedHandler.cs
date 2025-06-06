using BookingQueueSubscriber.Common.Logging;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;

namespace BookingQueueSubscriber.Services.MessageHandlers;

public class EndpointUpdatedHandler(
    IVideoApiService videoApiService,
    IVideoWebService videoWebService,
    ILogger<EndpointUpdatedHandler> logger)
    : IMessageHandler<EndpointUpdatedIntegrationEvent>
{
    private const int RetryLimit = 3;
    private const int RetrySleep = 3000;

    async Task IMessageHandler.HandleAsync(object integrationEvent)
    {
        await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
    }
        
    public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
    {
        var conference = await videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);

        if (conference == null)  
            logger.LogError("Unable to find conference by hearing id {HearingId}", eventMessage.HearingId);
        else
        {
            var linkedParticipants = await HandleEndpointParticipantsUpdate(conference, eventMessage);
            await videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip,
                new UpdateEndpointRequest
                {
                    DisplayName = eventMessage.DisplayName,
                    ParticipantsLinked = linkedParticipants.Select(e => e.Username).ToList(),
                    ConferenceRole = Enum.Parse<ConferenceRole>(eventMessage.Role.ToString())
                });

            var endpoints = await videoApiService.GetEndpointsForConference(conference.Id);

            var updateEndpointRequest = new UpdateConferenceEndpointsRequest
            {
                ExistingEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Sip).ToList()
            };

            await videoWebService.PushEndpointsUpdatedMessage(conference.Id, updateEndpointRequest);
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
    }

    private async Task<List<ParticipantResponse>> HandleEndpointParticipantsUpdate(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
    {
        var linkedParticipants = await FindLinkedParticipantsInConference(conference, endpointEvent);
        var endpoints = await videoApiService.GetEndpointsForConference(conference.Id);
        var endpointBeingUpdated = endpoints.SingleOrDefault(x => x.SipAddress == endpointEvent.Sip);
                
        try
        {
            if (endpointBeingUpdated is not null)
            {
                var newIds = linkedParticipants.Select(lp => lp.Id).ToHashSet();
                var existing = endpointBeingUpdated.ParticipantsLinked.ToHashSet(); 

                var newLinkedParticipants = linkedParticipants
                    .Where(lp => !existing.Contains(lp.Username)) 
                    .ToList();

                var linkedParticipantsToRemove = conference.Participants
                    .Where(ep => existing.Contains(ep.Username))
                    .ExceptBy(newIds, ep => ep.Id)            
                    .ToList();

                if (newLinkedParticipants.Count > 0 || linkedParticipantsToRemove.Count > 0)
                    await NotifyLinkedParticipants(conference, endpointBeingUpdated, linkedParticipants,newLinkedParticipants, linkedParticipantsToRemove);
            }
        }
        catch (Exception e)
        {
            logger.ErrorNotifyingLinkedParticipant(e);
        }
        return linkedParticipants;
    }

    private async Task NotifyLinkedParticipants(ConferenceDetailsResponse conference,
        EndpointResponse endpoint,
        List<ParticipantResponse> linkedParticipants,
        List<ParticipantResponse> newLinkedParticipants,
        List<ParticipantResponse> linkedParticipantsToRemove)
    {
        var tasks = new List<Task>();

        // Notify new linked participants
        if (newLinkedParticipants.Count > 0)
            tasks.AddRange(newLinkedParticipants.Select(participant => 
                videoWebService.PushLinkedNewParticipantToEndpoint(conference.Id, participant.Username, endpoint.DisplayName)));
            
        // Notify unlinked participants
        if (linkedParticipantsToRemove.Count > 0)
            tasks.AddRange(linkedParticipantsToRemove.Select(participant =>
                videoWebService.PushUnlinkedParticipantFromEndpoint(conference.Id, participant.Username, endpoint.DisplayName)));

        if (endpoint.Status == EndpointState.InConsultation)
        {
            //if old reps are in a private consultation with endpoint, and new/existing reps are not also present in the same room, force closure of the consultation
            var linkedParticipantsStillInConsultation = linkedParticipants.Any(p => IsParticipantIsInPrivateConsultationWithEndpoint(p, endpoint)); 
            var participantsToBootInConsultationWithEndpoint = linkedParticipantsToRemove
                .Where(p => IsParticipantIsInPrivateConsultationWithEndpoint(p, endpoint))
                .ToList();
                
            if (!linkedParticipantsStillInConsultation && participantsToBootInConsultationWithEndpoint.Count > 0)
            {
                tasks.AddRange(participantsToBootInConsultationWithEndpoint.Select(async participant =>
                {
                    await videoApiService.CloseConsultation(conference.Id, participant.Id);
                    await videoWebService.PushCloseConsultationBetweenEndpointAndParticipant(conference.Id, participant.Username, endpoint.DisplayName);
                }));
            }
                    
        }
        await Task.WhenAll(tasks);
    }
        
    private static bool IsParticipantIsInPrivateConsultationWithEndpoint(ParticipantResponse participant, EndpointResponse endpoint)
        => participant is not null && 
           participant.CurrentStatus == ParticipantState.InConsultation && 
           participant.CurrentRoom?.Id == endpoint.CurrentRoom?.Id;

    private async Task<List<ParticipantResponse>> FindLinkedParticipantsInConference(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
    {
        var participants = new List<ParticipantResponse>();
        if (endpointEvent?.ParticipantsLinked != null)
            foreach (var linkedParticipant in endpointEvent.ParticipantsLinked)
            {
                var participant = await QueryVideoApiForParticipantData(conference, linkedParticipant);
                participants.Add(participant);
            }

        return participants;
    }

    private async Task<ParticipantResponse> QueryVideoApiForParticipantData(ConferenceDetailsResponse conference, string linkedParticipant)
    {
        ParticipantResponse newLinkedParticipant = null;
        for (var retry = 0; retry <= RetryLimit; retry++)
        {
            newLinkedParticipant = GetLinkedParticipant(conference, linkedParticipant);
            if (newLinkedParticipant is not null)
                break;

            if (retry == RetryLimit)
                throw new ArgumentException(
                    $"Unable to find participant linked {linkedParticipant} from EndpointUpdatedIntegrationEvent in conference {conference.Id}");

            Thread.Sleep(RetrySleep);
            //refresh conference details
            conference = await videoApiService.GetConferenceByHearingRefId(conference.HearingId);
        }
        return newLinkedParticipant;
    }
        
    private static ParticipantResponse GetLinkedParticipant(ConferenceDetailsResponse conference, string linkedParticipant)
        => conference.Participants.SingleOrDefault(x => x.Username == linkedParticipant) ??
           conference.Participants.SingleOrDefault(x => x.ContactEmail == linkedParticipant);
        
        
}