using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
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
                var defenceAdvocate = await HandleEndpointDefenceAdvocateUpdate(conference, eventMessage);

                await videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip,
                    new UpdateEndpointRequest
                    {
                        DisplayName = eventMessage.DisplayName,
                        DefenceAdvocate = defenceAdvocate?.Username
                    });

                var endpoints = await videoApiService.GetEndpointsForConference(conference.Id);

                var updateEndpointRequest = new UpdateConferenceEndpointsRequest
                {
                    ExistingEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Sip).ToList()
                };

                await videoWebService.PushEndpointsUpdatedMessage(conference.Id, updateEndpointRequest);
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

        private async Task<ParticipantResponse> HandleEndpointDefenceAdvocateUpdate(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
        {
            var newDefenceAdvocate = await FindDefenceAdvocateInConference(conference, endpointEvent);
            var endpoints = await videoApiService.GetEndpointsForConference(conference.Id);
            var endpointBeingUpdated = endpoints.SingleOrDefault(x => x.SipAddress == endpointEvent.Sip);
                
            try
            {
                //Has the defence advocate changed?
                if (endpointBeingUpdated is not null &&!ReferenceEquals(endpointBeingUpdated.DefenceAdvocate, endpointEvent.DefenceAdvocate))
                    await NotifyDefenceAdvocates(conference, endpointEvent, newDefenceAdvocate, endpointBeingUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error notifying defence advocates");
            }
            return newDefenceAdvocate;
        }

        private async Task<ParticipantResponse> FindDefenceAdvocateInConference(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
        {
            ParticipantResponse newDefenceAdvocate = null;

            if (!string.IsNullOrEmpty(endpointEvent.DefenceAdvocate))
            {
                for (var retry = 0; retry <= RetryLimit; retry++)
                {
                    newDefenceAdvocate = GetDefenceAdvocate(conference, endpointEvent.DefenceAdvocate);
                    if (newDefenceAdvocate is not null)
                        break;

                    if (retry == RetryLimit)
                        throw new ArgumentException(
                            $"Unable to find defence advocate {endpointEvent.DefenceAdvocate} from EndpointUpdatedIntegrationEvent in conference {conference.Id}");

                    Thread.Sleep(RetrySleep);
                    //refresh conference details
                    conference = await videoApiService.GetConferenceByHearingRefId(conference.HearingId);
                }
            }

            return newDefenceAdvocate;
        }

        private async Task NotifyDefenceAdvocates(ConferenceDetailsResponse conference, 
            EndpointUpdatedIntegrationEvent endpointEvent, 
            ParticipantResponse newDefenceAdvocate, 
            EndpointResponse endpointBeingUpdated)
        {
            //Has a new rep been linked
            if (newDefenceAdvocate is not null)
                await videoWebService.PushLinkedNewParticipantToEndpoint(conference.Id, newDefenceAdvocate.Username, endpointEvent.DisplayName);

            //Was there a previously linked Rep
            if (endpointBeingUpdated.DefenceAdvocate is not null)
            {
                var previousDefenceAdvocateName = endpointBeingUpdated.DefenceAdvocate;
                var oldDefenceAdvocate = GetDefenceAdvocate(conference, previousDefenceAdvocateName) 
                    ?? throw new ArgumentException($"Unable to find defence advocate in participant list {previousDefenceAdvocateName}");
                
                await videoWebService.PushUnlinkedParticipantFromEndpoint(conference.Id, oldDefenceAdvocate.Username, endpointEvent.DisplayName);

                //if old rep is in a private consultation with endpoint, and new rep is not also present in the same room, force closure of the consultation
                if (endpointBeingUpdated.Status == EndpointState.InConsultation &&
                    !IsParticipantIsInPrivateConsultationWithEndpoint(newDefenceAdvocate, endpointBeingUpdated) &&
                    IsParticipantIsInPrivateConsultationWithEndpoint(oldDefenceAdvocate, endpointBeingUpdated))
                {
                    await videoApiService.CloseConsultation(conference.Id, oldDefenceAdvocate.Id);
                    await videoWebService.PushCloseConsultationBetweenEndpointAndParticipant(conference.Id, oldDefenceAdvocate.Username, endpointEvent.DisplayName);
                }
            }
        }

        private static ParticipantResponse GetDefenceAdvocate(ConferenceDetailsResponse conference, string defenceAdvocate)
        {
            return conference.Participants.SingleOrDefault(x => x.Username == defenceAdvocate) ??
                   conference.Participants.SingleOrDefault(x => x.ContactEmail == defenceAdvocate);
        }
        
        private static bool IsParticipantIsInPrivateConsultationWithEndpoint(ParticipantResponse participant, EndpointResponse endpoint)
        {
            return participant is not null &&
                   participant.CurrentStatus == ParticipantState.InConsultation &&
                   participant.CurrentRoom?.Id == endpoint.CurrentRoom?.Id;
        }
    }
}