using BookingQueueSubscriber.Services.VideoApi;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointAddedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService)
        : IMessageHandler<EndpointAddedIntegrationEvent>
    {
        public async Task HandleAsync(EndpointAddedIntegrationEvent eventMessage)
        {
            var conference = await videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            ParticipantResponse defenceAdvocate = null;
            if (!string.IsNullOrEmpty(eventMessage.Endpoint.DefenceAdvocateContactEmail))
            {
                defenceAdvocate = conference.Participants.Single(x => x.ContactEmail ==
                    eventMessage.Endpoint.DefenceAdvocateContactEmail);
            }

            await videoApiService.AddEndpointToConference(conference.Id, new AddEndpointRequest
            {
                DisplayName = eventMessage.Endpoint.DisplayName,
                SipAddress = eventMessage.Endpoint.Sip,
                Pin = eventMessage.Endpoint.Pin,
                DefenceAdvocate = defenceAdvocate?.Username
            });

            var endpoints = await videoApiService.GetEndpointsForConference(conference.Id);

            // We are only ever going to have one Endpoint at a time
            // However, sending as a list will allow for bulk items to be sent in the future
            var addEndpointRequest = new UpdateConferenceEndpointsRequest
            {
                NewEndpoints = endpoints.Where(x => x.SipAddress == eventMessage.Endpoint.Sip).ToList()
            };

            await videoWebService.PushEndpointsUpdatedMessage(conference.Id, addEndpointRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointAddedIntegrationEvent)integrationEvent);
        }
    }
}