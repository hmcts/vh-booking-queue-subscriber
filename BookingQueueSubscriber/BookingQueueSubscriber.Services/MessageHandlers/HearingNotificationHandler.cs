using BookingQueueSubscriber.Services.NotificationApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingNotificationHandler : IMessageHandler<HearingNotificationIntegrationEvent>
    {
        private readonly INotificationService _notificationServiceService;

        public HearingNotificationHandler(INotificationService notificationService)
        {
            _notificationServiceService = notificationService;
        }

        public async Task HandleAsync(HearingNotificationIntegrationEvent eventMessage)
        {
            await _notificationServiceService.SendNewSingleDayHearingConfirmationNotification(eventMessage.Hearing,
                eventMessage.Participants);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingNotificationIntegrationEvent)integrationEvent);
        }
    }
}