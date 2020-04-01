using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDetailsUpdatedHandler : IMessageHandler<HearingDetailsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingDetailsUpdatedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingDetailsUpdatedIntegrationEvent eventMessage)
        {
            var request = HearingToUpdateConferenceMapper.MapToUpdateConferenceRequest(eventMessage.Hearing);

            await _videoApiService.UpdateConferenceAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDetailsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}