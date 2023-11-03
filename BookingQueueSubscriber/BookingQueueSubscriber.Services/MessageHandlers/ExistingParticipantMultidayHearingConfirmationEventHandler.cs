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
            var cleanedCaseName = message.CaseName.Replace($"Day 1 of {eventMessage.TotalDays}", string.Empty).Trim();
            var request = new ExistingUserMultiDayHearingConfirmationRequest
            {
                Name = $"{message.FirstName} {message.LastName}",
                CaseName = cleanedCaseName,
                CaseNumber = message.CaseNumber,
                ContactEmail = message.ContactEmail,
                DisplayName = message.DisplayName,
                HearingId = message.HearingId,
                ParticipantId = message.ParticipantId,
                Representee = message.Representee,
                RoleName = message.Username,
                ScheduledDateTime = message.ScheduledDateTime,
                TotalDays = eventMessage.TotalDays,
                Username = message.Username
            };

            await _notificationApiClient.SendParticipantMultiDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ExistingParticipantMultidayHearingConfirmationEvent)integrationEvent);
        }
    }
}