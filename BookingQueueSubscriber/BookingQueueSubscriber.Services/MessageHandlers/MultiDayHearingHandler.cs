using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class MultiDayHearingHandler : IMessageHandler<MultiDayHearingIntegrationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public MultiDayHearingHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(MultiDayHearingIntegrationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var request = NotificationRequestHelper.BuildExistingUserMultiDayHearingConfirmationRequest(message, eventMessage.TotalDays);

            await _notificationApiClient.SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        public async Task HandleAsync(object integrationEvent)
        {
            await HandleAsync((MultiDayHearingIntegrationEvent)integrationEvent);
        }
    }
}