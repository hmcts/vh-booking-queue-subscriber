using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

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
            
            await _videoApiService.AddEndpointToConference(conference.Id, new AddEndpointRequest
            {
                DisplayName = eventMessage.Endpoint.DisplayName,
                SipAddress = eventMessage.Endpoint.Sip,
                Pin = eventMessage.Endpoint.Pin
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointAddedIntegrationEvent)integrationEvent);
        }
    }
}