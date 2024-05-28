using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointUpdatedHandler : IMessageHandler<EndpointUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly ILogger<EndpointUpdatedHandler> _logger;
        private const int RetryLimit = 3;
        private const int RetrySleep = 3000;
        public EndpointUpdatedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, ILogger<EndpointUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _logger = logger;
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
        
        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);

            if (conference == null)
                _logger.LogError("Unable to find conference by hearing id {HearingId}", eventMessage.HearingId);
            else
            {
                await HandleLinkedParticipantUpdate(conference, eventMessage);
                
                await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip, new UpdateEndpointRequest { DisplayName = eventMessage.DisplayName });

                var endpoints = await _videoApiService.GetEndpointsForConference(conference.Id);

                var updateEndpointRequest = new UpdateConferenceEndpointsRequest
                {
                    ExistingEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Sip).ToList()
                };

                await _videoWebService.PushEndpointsUpdatedMessage(conference.Id, updateEndpointRequest);
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

        private async Task HandleLinkedParticipantUpdate(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
        {
            var endpoints = await _videoApiService.GetEndpointsForConference(conference.Id);
            var endpointBeingUpdated = endpoints.SingleOrDefault(x => x.SipAddress == endpointEvent.Sip);
                
            try
            {
                //Has the defence advocate changed?
                if (endpointBeingUpdated is not null)
                {
                    //Push the new linked participant notification to VideoWeb
                    foreach (var newLinkedParticipant in endpointEvent.EndpointParticipantsAdded)
                        await _videoWebService.PushLinkedNewParticipantToEndpoint(conference.Id, newLinkedParticipant, endpointEvent.DisplayName);
                    
                    await NotifyExistingParticipants(conference, endpointEvent, endpointBeingUpdated);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error notifying defence advocates");
            }
        }

        private async Task NotifyExistingParticipants(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent, EndpointResponse endpointBeingUpdated)
        {
            foreach (var participantBeingRemoved in endpointEvent.EndpointParticipantsRemoved)
            {
                await _videoWebService.PushUnlinkedParticipantFromEndpoint(conference.Id, participantBeingRemoved, endpointEvent.DisplayName);
                
                var oldParticipant = GetParticipant(conference, participantBeingRemoved);

                //if old rep is in a private consultation with endpoint, check if another linked rep is there, else, force closure of the consultation
                if (endpointBeingUpdated.Status == EndpointState.InConsultation && IsParticipantIsInPrivateConsultationWithEndpoint(oldParticipant, endpointBeingUpdated))
                {
                    var otherEndpointParticipants = endpointEvent.EndpointParticipants.Select(x => GetParticipant(conference, x));
                    if(otherEndpointParticipants.Any(x => IsParticipantIsInPrivateConsultationWithEndpoint(x, endpointBeingUpdated)))
                        continue;   
                    await _videoApiService.CloseConsultation(conference.Id, oldParticipant.Id);
                    await _videoWebService.PushCloseConsultationBetweenEndpointAndParticipant(conference.Id, oldParticipant.Username, endpointEvent.DisplayName);
                }
            }
        }

        private static ParticipantDetailsResponse GetParticipant(ConferenceDetailsResponse conference, string username)
        {
            return conference.Participants.SingleOrDefault(x => x.Username == username) ??
                   conference.Participants.SingleOrDefault(x => x.ContactEmail == username);
        }
        
        private static bool IsParticipantIsInPrivateConsultationWithEndpoint(ParticipantDetailsResponse participant, EndpointResponse endpoint)
        {
            return participant is not null &&
                   participant.CurrentStatus == ParticipantState.InConsultation &&
                   participant.CurrentRoom?.Id == endpoint.CurrentRoom?.Id;
        }
    }
}