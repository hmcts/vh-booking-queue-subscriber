using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantMultidayHearingConfirmationHandler : IMessageHandler<NewParticipantMultidayHearingConfirmationEvent>
    {
        private readonly IUserService _userService;
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;

        public NewParticipantMultidayHearingConfirmationHandler(IUserService userService,
            INotificationApiClient notificationApiClient,
            IBookingsApiClient bookingsApiClient)
        {
            _userService = userService;
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
        }

        public async Task HandleAsync(NewParticipantMultidayHearingConfirmationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                message.LastName, message.ContactEmail, false);
            var cleanedCaseName = message.CaseName.Replace($"Day 1 of {eventMessage.TotalDays}", string.Empty).Trim();

            message.Username = newUser.UserName;

            var request = new AddNotificationRequest
            {
                HearingId = message.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = message.ContactEmail,
                NotificationType = NotificationType.NewUserLipConfirmationMultiDay,
                ParticipantId = message.ParticipantId,
                PhoneNumber = message.ContactTelephone,
                Parameters = new Dictionary<string, string>
                {
                    { NotifyParams.Name, $"{message.FirstName} {message.LastName}" },
                    { NotifyParams.CaseName, cleanedCaseName },
                    { NotifyParams.CaseNumber, message.CaseNumber },

                    { NotifyParams.StartDayMonthYear, message.ScheduledDateTime.ToEmailDateGbLocale() },
                    { NotifyParams.Time,message.ScheduledDateTime.ToEmailTimeGbLocale() },
                    { NotifyParams.DayMonthYear, message.ScheduledDateTime.ToEmailDateGbLocale() },
                    { NotifyParams.DayMonthYearCy, message.ScheduledDateTime.ToEmailDateCyLocale() },
                    { NotifyParams.StartTime, message.ScheduledDateTime.ToEmailTimeGbLocale() },
                    { NotifyParams.UserName, message.Username.ToLower() },
                    { NotifyParams.RandomPassword, newUser.Password },
                    { NotifyParams.NumberOfDays, eventMessage.TotalDays.ToString() }
                }
            };

            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _notificationApiClient.CreateNewNotificationAsync(request);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantMultidayHearingConfirmationEvent)integrationEvent);
        }
    }
}