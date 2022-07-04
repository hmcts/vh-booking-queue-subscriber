using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingNotificationHandler : IMessageHandler<HearingNotificationIntegrationEvent>
    {
        private readonly IUserCreationAndNotification _userCreationAndNotification;

        public HearingNotificationHandler(IUserCreationAndNotification userCreationAndNotification)
        {
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(HearingNotificationIntegrationEvent eventMessage)
        {
            await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, eventMessage.Participants);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingNotificationIntegrationEvent)integrationEvent);
        }
    }
}