using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingCancelledHandler : IMessageHandler<HearingCancelledIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingCancelledHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public Task HandleAsync(HearingCancelledIntegrationEvent eventMessage)
        {
            throw new System.NotImplementedException();
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingCancelledIntegrationEvent)integrationEvent);
        }
    }
}