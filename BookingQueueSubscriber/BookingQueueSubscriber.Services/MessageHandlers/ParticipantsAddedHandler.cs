using BookingQueueSubscriber.Common.Logging;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers;

public class ParticipantsAddedHandler(
    IVideoApiService videoApiService,
    IVideoWebService videoWebService,
    ILogger<ParticipantsAddedHandler> logger)
    : IMessageHandler<ParticipantsAddedIntegrationEvent>
{
    public async Task HandleAsync(ParticipantsAddedIntegrationEvent eventMessage)
    {
        var conference = await videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId);
        logger.UpdatingParticipantList(conference.Id);
        var request = new AddParticipantsToConferenceRequest
        {
            Participants = eventMessage.Participants
                .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList()
        };

        await videoApiService.AddParticipantsToConference(conference.Id, request);

        conference = await videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId);
        var newParticipants = ExtractNewParticipants(conference, eventMessage);

        var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
        {
            NewParticipants = newParticipants.ToList()
        };
        await videoWebService.PushParticipantsUpdatedMessage(conference.Id, updateConferenceParticipantsRequest);
    }

    private static IEnumerable<ParticipantRequest> ExtractNewParticipants(ConferenceDetailsResponse conference, 
        ParticipantsAddedIntegrationEvent eventMessage)
    {
        var addedParticipants = conference.Participants
            .Where(participant => eventMessage.Participants.Any(p => p.ParticipantId == participant.RefId))
            .ToList();

        var newParticipants = new List<ParticipantRequest>();
        var eventMessageParticipants = eventMessage.Participants.ToList();
            
        foreach (var eventMessageParticipant in eventMessageParticipants)
        {
            var participantId = eventMessageParticipant.ParticipantId;
            var participantRefId = eventMessageParticipant.ParticipantId;
                
            var participant = addedParticipants.SingleOrDefault(x => x.RefId == eventMessageParticipant.ParticipantId);
            if (participant != null)
            {
                participantId = participant.Id;
                participantRefId = participant.RefId;
            }
                
            var newParticipant = ParticipantToParticipantRequestMapper.MapToParticipantRequest(eventMessageParticipant, participantId, participantRefId);
            newParticipants.Add(newParticipant);
        }

        return newParticipants;
    }

    async Task IMessageHandler.HandleAsync(object integrationEvent)
    {
        await HandleAsync((ParticipantsAddedIntegrationEvent)integrationEvent);
    }
}