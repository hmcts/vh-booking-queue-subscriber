using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingCancelledHandler : IMessageHandler<HearingCancelledIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingCancelledHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingCancelledIntegrationEvent eventMessage)
        {
            var conferenceDto = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            await _videoApiService.DeleteConferenceAsync(conferenceDto.Id);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingCancelledIntegrationEvent)integrationEvent);
        }
    }
}