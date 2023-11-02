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

            var request = new AddNotificationRequest
            {
                HearingId = message.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = message.ContactEmail,
                NotificationType = NotificationType.ExistingUserLipConfirmation,
                ParticipantId = message.ParticipantId,
                PhoneNumber = message.ContactTelephone,
                Parameters = new Dictionary<string, string>
                {
                    {NotifyParams.Name, $"{message.FirstName} {message.LastName}" },
                    {NotifyParams.CaseName, message.CaseName },
                    {NotifyParams.CaseNumber, message.CaseNumber },

                    {NotifyParams.DayMonthYear,message.ScheduledDateTime.ToEmailDateGbLocale() },
                    {NotifyParams.DayMonthYearCy,message.ScheduledDateTime.ToEmailDateCyLocale() },

                    {NotifyParams.StartTime,message.ScheduledDateTime.ToEmailTimeGbLocale() },
                    {NotifyParams.UserName,message.Username.ToLower() },
                }
            };

            await _notificationApiClient.CreateNewNotificationAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ExistingParticipantHearingConfirmationEvent)integrationEvent);
        }
    }

}