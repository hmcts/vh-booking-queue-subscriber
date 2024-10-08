using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDetailsUpdatedHandler : IMessageHandler<HearingDetailsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public HearingDetailsUpdatedHandler(IVideoApiService videoApiService,
            IVideoWebService videoWebService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(HearingDetailsUpdatedIntegrationEvent eventMessage)
        {
            var request = HearingToUpdateConferenceMapper.MapToUpdateConferenceRequest(eventMessage.Hearing);

            await _videoApiService.UpdateConferenceAsync(request);
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId);
            await _videoWebService.PushHearingDetailsUpdatedMessage(conference.Id);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDetailsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}