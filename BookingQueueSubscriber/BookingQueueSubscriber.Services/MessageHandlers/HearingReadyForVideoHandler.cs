using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants);

            await _videoApiService.BookNewConferenceAsync(request).ConfigureAwait(false);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}