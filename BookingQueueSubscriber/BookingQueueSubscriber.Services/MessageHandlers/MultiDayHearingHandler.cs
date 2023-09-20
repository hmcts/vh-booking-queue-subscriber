using BookingQueueSubscriber.Services.NotificationApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class MultiDayHearingHandler : IMessageHandler<MultiDayHearingIntegrationEvent>
    {
        private readonly INotificationService _notificationService;

        public MultiDayHearingHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(MultiDayHearingIntegrationEvent eventMessage)
        {
            await _notificationService.SendNewMultiDayHearingConfirmationNotificationAsync(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.TotalDays);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((MultiDayHearingIntegrationEvent)integrationEvent);
        }
    }
}