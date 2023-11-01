using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantHearingConfirmationHandler : IMessageHandler<NewParticipantHearingConfirmationEvent>
    {
        private readonly IUserService _userService;
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;

        public NewParticipantHearingConfirmationHandler(IUserService userService,
            INotificationApiClient notificationApiClient,
            IBookingsApiClient bookingsApiClient)
        {
            _userService = userService;
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient; 
        }

        public async Task HandleAsync(NewParticipantHearingConfirmationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                message.LastName, message.ContactEmail, false);

            message.Username = newUser.UserName;

            var request = new AddNotificationRequest
            {
                HearingId = message.HearingId,
                MessageType = MessageType.Email,
                ContactEmail = message.ContactEmail,
                NotificationType = NotificationType.NewUserLipConfirmation,
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
                    {NotifyParams.RandomPassword, newUser.Password }
                }
            };

            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _notificationApiClient.CreateNewNotificationAsync(request);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
        }
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantHearingConfirmationEvent)integrationEvent);
        }
    }
}