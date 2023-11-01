using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Client;
using NotificationApi.Contract;
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
                    { NotifyParams.Name, $"{message.FirstName} {message.LastName}" },
                    { NotifyParams.CaseName, cleanedCaseName },
                    { NotifyParams.CaseNumber, message.CaseNumber },

                    { NotifyParams.StartDayMonthYear, message.ScheduledDateTime.ToEmailDateGbLocale()},
                    { NotifyParams.Time,message.ScheduledDateTime.ToEmailTimeGbLocale() },
                    { NotifyParams.DayMonthYear, message.ScheduledDateTime.ToEmailDateGbLocale() },
                    { NotifyParams.DayMonthYearCy, message.ScheduledDateTime.ToEmailDateCyLocale() },
                    { NotifyParams.StartTime, message.ScheduledDateTime.ToEmailTimeGbLocale() },
                    { NotifyParams.UserName, message.Username.ToLower() },
                    { NotifyParams.NumberOfDays, eventMessage.TotalDays.ToString() }
                }
            };

            await _notificationApiClient.CreateNewNotificationAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ExistingParticipantMultidayHearingConfirmationEvent)integrationEvent);
        }
    }
}