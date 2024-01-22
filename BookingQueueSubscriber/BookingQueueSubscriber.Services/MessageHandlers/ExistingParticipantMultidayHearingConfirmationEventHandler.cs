using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ExistingParticipantMultidayHearingConfirmationEventHandler : IMessageHandler<ExistingParticipantMultidayHearingConfirmationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public ExistingParticipantMultidayHearingConfirmationEventHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(ExistingParticipantMultidayHearingConfirmationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var request = NotificationRequestHelper.BuildExistingUserMultiDayHearingConfirmationRequest(message, eventMessage.TotalDays);

            await _notificationApiClient.SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ExistingParticipantMultidayHearingConfirmationEvent)integrationEvent);
        }
    }
}