using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingNotificationHandler : IMessageHandler<HearingNotificationIntegrationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public HearingNotificationHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(HearingNotificationIntegrationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;

            var request = NotificationRequestHelper.BuildExistingUserSingleDayHearingConfirmationRequest(message);

            await _notificationApiClient.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingNotificationIntegrationEvent)integrationEvent);
        }
    }
}