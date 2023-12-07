using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateAndNotifyUserHandler : IMessageHandler<CreateAndNotifyUserIntegrationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;

        public CreateAndNotifyUserHandler(IUserService userService,
            INotificationApiClient notificationApiClient,
            IBookingsApiClient bookingsApiClient)
        {
            _userService = userService;
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
        }

        public async Task HandleAsync(CreateAndNotifyUserIntegrationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                    message.LastName, message.ContactEmail, false);

            message.Username = newUser.UserName;

            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
            
            await _notificationApiClient.SendParticipantCreatedAccountEmailAsync(new SignInDetailsEmailRequest
            {
                ContactEmail = message.ContactEmail,
                Name = $"{message.FirstName} {message.LastName}",
                RoleName = message.UserRole,
                Username = message.Username,
                Password = newUser.Password,
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateAndNotifyUserIntegrationEvent)integrationEvent);
        }
    }
}