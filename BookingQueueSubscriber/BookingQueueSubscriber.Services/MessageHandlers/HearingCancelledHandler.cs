using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingCancelledHandler : IMessageHandler<HearingCancelledIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public HearingCancelledHandler(IVideoApiService videoApiService,
            IVideoWebService videoWebService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(HearingCancelledIntegrationEvent eventMessage)
        {
            var conferenceDto = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            await _videoApiService.DeleteConferenceAsync(conferenceDto.Id);
            await _videoWebService.PushHearingCancelledMessage(conferenceDto.Id);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingCancelledIntegrationEvent)integrationEvent);
        }
    }
}