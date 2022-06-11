using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDateTimeChangedHandler : IMessageHandler<HearingDateTimeChangedIntegrationEvent>
    {
        private readonly INotificationService _notificationService;

        public HearingDateTimeChangedHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingDateTimeChangedIntegrationEvent eventMessage)
        {
            await _notificationService.SendHearingAmendmentNotificationAsync(eventMessage.Hearing,
                eventMessage.OldScheduledDateTime, eventMessage.Participants);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDateTimeChangedIntegrationEvent)integrationEvent);
        }
    }
}