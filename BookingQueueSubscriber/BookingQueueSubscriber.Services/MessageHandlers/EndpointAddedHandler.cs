using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using VideoApi.Contract.Requests;
using System.Linq;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointAddedHandler : IMessageHandler<EndpointAddedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public EndpointAddedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(EndpointAddedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            ParticipantDetailsResponse defenceAdvocate = null;
            if (!string.IsNullOrEmpty(eventMessage.Endpoint.DefenceAdvocateContactEmail))
            {
                defenceAdvocate = conference.Participants.Single(x => x.ContactEmail ==
                    eventMessage.Endpoint.DefenceAdvocateContactEmail);
            }

            await _videoApiService.AddEndpointToConference(conference.Id, new AddEndpointRequest
            {
                DisplayName = eventMessage.Endpoint.DisplayName,
                SipAddress = eventMessage.Endpoint.Sip,
                Pin = eventMessage.Endpoint.Pin,
                DefenceAdvocate = defenceAdvocate?.Username
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointAddedIntegrationEvent)integrationEvent);
        }
    }
}