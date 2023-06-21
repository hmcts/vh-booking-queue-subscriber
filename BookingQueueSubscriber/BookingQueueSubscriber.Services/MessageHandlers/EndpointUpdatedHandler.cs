using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
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

        public EndpointUpdatedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, ILogger<EndpointUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _logger = logger;
        }

        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);

            if (conference == null)
                _logger.LogError("Unable to find conference by hearing id {HearingId}", eventMessage.HearingId);
            else
            {
                var defenceAdvocate = await HandleEndpointDefenceAdvocateUpdate(conference, eventMessage);

                await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip,
                    new UpdateEndpointRequest
                    {
                        DisplayName = eventMessage.DisplayName,
                        DefenceAdvocate = defenceAdvocate?.Username
                    });

                var endpoints = await _videoApiService.GetEndpointsForConference(conference.Id);

                var updateEndpointRequest = new UpdateConferenceEndpointsRequest
                {
                    ExistingEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Sip).ToList()
                };

                await _videoWebService.PushEndpointsUpdatedMessage(conference.Id, updateEndpointRequest);
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

        private async Task<ParticipantDetailsResponse> HandleEndpointDefenceAdvocateUpdate(ConferenceDetailsResponse conference, EndpointUpdatedIntegrationEvent endpointEvent)
        {
            ParticipantDetailsResponse newDefenceAdvocate = null;
            if (!String.IsNullOrEmpty(endpointEvent.DefenceAdvocate))
                newDefenceAdvocate = GetDefenceAdvocate(conference, endpointEvent.DefenceAdvocate);
            
            var endpoints = await _videoApiService.GetEndpointsForConference(conference.Id);
            var endpointBeingUpdated = endpoints.SingleOrDefault(x => x.SipAddress == endpointEvent.Sip);
            
            //Has the defence advocate changed?
            if (endpointBeingUpdated is not null &&!ReferenceEquals(endpointBeingUpdated.DefenceAdvocate, endpointEvent.DefenceAdvocate))
                await NotifyDefenceAdvocates(conference, endpointEvent, newDefenceAdvocate, endpointBeingUpdated);
            
            return newDefenceAdvocate;
        }

        private async Task NotifyDefenceAdvocates(ConferenceDetailsResponse conference, 
            EndpointUpdatedIntegrationEvent endpointEvent, 
            ParticipantDetailsResponse newDefenceAdvocate, 
            EndpointResponse endpointBeingUpdated)
        {
            //Has a new rep been linked
            if (newDefenceAdvocate is not null)
                await _videoWebService.PushLinkedNewParticipantToEndpoint(conference.Id, newDefenceAdvocate.Username, endpointEvent.DisplayName);

            //Was there a previously linked Rep
            if (endpointBeingUpdated.DefenceAdvocate is not null)
            {
                var oldDefenceAdvocate = GetDefenceAdvocate(conference, endpointBeingUpdated.DefenceAdvocate);
                await _videoWebService.PushUnlinkedParticipantFromEndpoint(conference.Id, oldDefenceAdvocate.Username, endpointEvent.DisplayName);

                //if old rep is in a private consultation with endpoint, and new rep is not also present in the same room, force closure of the consultation
                if (endpointBeingUpdated.Status == EndpointState.InConsultation &&
                    !IsParticipantIsInPrivateConsultationWithEndpoint(newDefenceAdvocate, endpointBeingUpdated) &&
                    IsParticipantIsInPrivateConsultationWithEndpoint(oldDefenceAdvocate, endpointBeingUpdated))
                {
                    await _videoApiService.CloseConsultation(conference.Id, oldDefenceAdvocate.Id);
                    await _videoWebService.PushCloseConsultationBetweenEndpointAndParticipant(conference.Id, oldDefenceAdvocate.Username, endpointEvent.DisplayName);
                }
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
        
        private static ParticipantDetailsResponse GetDefenceAdvocate(ConferenceDetailsResponse conference, string contactEmail) =>
            conference.Participants.SingleOrDefault(x => x.ContactEmail == contactEmail);
        
        private static bool IsParticipantIsInPrivateConsultationWithEndpoint(ParticipantDetailsResponse participant, EndpointResponse endpoint)
        {
            return participant is not null &&
                   participant.CurrentStatus == ParticipantState.InConsultation &&
                   participant.CurrentRoom?.Id == endpoint.CurrentRoom?.Id;
        }
    }
}