using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDateTimeChangedHandler : IMessageHandler<HearingDateTimeChangedIntegrationEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IVideoWebService _videoWebService;

        public HearingDateTimeChangedHandler(INotificationService notificationService,
            IVideoWebService videoWebService)
        {
            _notificationService = notificationService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(HearingDateTimeChangedIntegrationEvent eventMessage)
        {
            await _notificationService.SendHearingAmendmentNotificationAsync(eventMessage.Hearing,
                eventMessage.OldScheduledDateTime, eventMessage.Participants);
            await _videoWebService.PushHearingDateTimeChangedMessage(eventMessage.Hearing.HearingId);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDateTimeChangedIntegrationEvent)integrationEvent);
        }
    }
}