using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

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
            var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(
                eventMessage.Hearing, eventMessage.Participants);

            await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateAndNotifyUserIntegrationEvent)integrationEvent);
        }
    }
}