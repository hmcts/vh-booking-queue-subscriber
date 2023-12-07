using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingAmendmentNotificationHandler : IMessageHandler<HearingAmendmentNotificationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public HearingAmendmentNotificationHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(HearingAmendmentNotificationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;

            var request = new HearingAmendmentRequest
            {
                HearingId = message.HearingId,
                ContactEmail = message.ContactEmail,
                ParticipantId = message.ParticipantId,
                CaseName = message.CaseName,
                PreviousScheduledDateTime = message.ScheduledDateTime,
                NewScheduledDateTime = eventMessage.NewScheduledDateTime,
                RoleName = message.UserRole,
                CaseNumber = message.CaseNumber,
                Name = $"{message.FirstName} {message.LastName}",
                DisplayName = message.DisplayName,
                Representee = message.Representee,
                Username = message.Username
            };

            await _notificationApiClient.SendHearingAmendmentEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingAmendmentNotificationEvent)integrationEvent);
        }
    }
}