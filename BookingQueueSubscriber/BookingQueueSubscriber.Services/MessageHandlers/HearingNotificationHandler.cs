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

            var request = new ExistingUserSingleDayHearingConfirmationRequest
            {
                HearingId = message.HearingId,
                ContactEmail = message.ContactEmail,
                ParticipantId = message.ParticipantId,
                CaseName = message.CaseName,
                CaseNumber = message.CaseNumber,
                DisplayName = message.DisplayName,
                Name = $"{message.FirstName} {message.LastName}",
                Representee = message.Representee,
                Username = message.Username,
                RoleName = message.UserRole,
                ScheduledDateTime = message.ScheduledDateTime
            };

            await _notificationApiClient.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingNotificationIntegrationEvent)integrationEvent);
        }
    }
}