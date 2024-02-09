using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateAndNotifyUserHandler : IMessageHandler<CreateAndNotifyUserIntegrationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IHearingService _hearingService;

        public CreateAndNotifyUserHandler(INotificationApiClient notificationApiClient,
            IHearingService hearingService)
        {
            _notificationApiClient = notificationApiClient;
            _hearingService = hearingService;
        }

        public async Task HandleAsync(CreateAndNotifyUserIntegrationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            
            var newUser = await _hearingService.CreateUserForHearing(message.HearingId, 
                message.FirstName, message.LastName, message.ContactEmail, message.UserRole);
            await _notificationApiClient.SendParticipantCreatedAccountEmailAsync(new SignInDetailsEmailRequest
            {
                ContactEmail = message.ContactEmail,
                Name = $"{message.FirstName} {message.LastName}",
                RoleName = message.UserRole,
                Username = newUser.UserName,
                Password = newUser.Password,
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateAndNotifyUserIntegrationEvent)integrationEvent);
        }
    }
}