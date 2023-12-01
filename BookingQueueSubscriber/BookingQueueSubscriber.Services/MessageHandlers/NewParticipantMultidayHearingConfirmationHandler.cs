using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using NotificationApi.Client;
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

            var request = new NewUserMultiDayHearingConfirmationRequest
            {
                HearingId = message.HearingId,
                ContactEmail = message.ContactEmail,
                ParticipantId = message.ParticipantId,
                Name = $"{message.FirstName} {message.LastName}",
                CaseName = cleanedCaseName,
                CaseNumber = message.CaseNumber,
                RandomPassword = newUser.Password,
                RoleName = message.UserRole,
                ScheduledDateTime = message.ScheduledDateTime,
                TotalDays = eventMessage.TotalDays,
                Username = message.Username
            };

            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _notificationApiClient.SendParticipantMultiDayHearingConfirmationForNewUserEmailAsync(request);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantMultidayHearingConfirmationEvent)integrationEvent);
        }
    }
}