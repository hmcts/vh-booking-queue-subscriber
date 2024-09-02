using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDateTimeChangedHandler : IMessageHandler<HearingDateTimeChangedIntegrationEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public HearingDateTimeChangedHandler(INotificationService notificationService,
            IVideoApiService videoApiService,
            IVideoWebService videoWebService)
        {
            _notificationService = notificationService;
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(HearingDateTimeChangedIntegrationEvent eventMessage)
        {
            await _notificationService.SendHearingAmendmentNotificationAsync(eventMessage.Hearing,
                eventMessage.OldScheduledDateTime, eventMessage.Participants);
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId);
            await _videoWebService.PushHearingDateTimeChangedMessage(conference.Id);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDateTimeChangedIntegrationEvent)integrationEvent);
        }
    }
}