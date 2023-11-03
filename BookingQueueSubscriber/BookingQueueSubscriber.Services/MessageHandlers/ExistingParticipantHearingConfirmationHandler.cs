using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ExistingParticipantHearingConfirmationHandler : IMessageHandler<ExistingParticipantHearingConfirmationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;

        public ExistingParticipantHearingConfirmationHandler(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(ExistingParticipantHearingConfirmationEvent eventMessage)
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
            await HandleAsync((ExistingParticipantHearingConfirmationEvent)integrationEvent);
        }
    }

}