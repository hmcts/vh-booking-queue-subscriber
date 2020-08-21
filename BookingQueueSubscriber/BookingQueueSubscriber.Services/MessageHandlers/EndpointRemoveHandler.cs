using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointRemoveHandler : IMessageHandler<EndpointRemovedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public EndpointRemoveHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(EndpointRemovedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            
            await _videoApiService.RemoveEndpointFromConference(conference.Id, eventMessage.EndpointId);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointRemovedIntegrationEvent)integrationEvent);
        }
    }
}