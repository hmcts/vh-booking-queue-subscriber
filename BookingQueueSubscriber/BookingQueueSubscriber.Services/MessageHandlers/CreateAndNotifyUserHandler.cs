namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateAndNotifyUserHandler : IMessageHandler<CreateAndNotifyUserIntegrationEvent>
    {
        private readonly IUserCreationAndNotification _userCreationAndNotification;

        public CreateAndNotifyUserHandler(IUserCreationAndNotification userCreationAndNotification)
        {
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(CreateAndNotifyUserIntegrationEvent eventMessage)
        {
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndSendNotificationAsync(
                eventMessage.Hearing, eventMessage.Participants);

            await _userCreationAndNotification.AssignUserToGroupForHearing(newParticipantUsers);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateAndNotifyUserIntegrationEvent)integrationEvent);
        }
    }
}