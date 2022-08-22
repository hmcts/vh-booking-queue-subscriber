using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class JudgeUpdatedHandler : IMessageHandler<JudgeUpdatedIntegrationEvent>
    {
        private readonly IUserCreationAndNotification _userCreationAndNotification;

        public JudgeUpdatedHandler(  IUserCreationAndNotification userCreationAndNotification)
        {
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(JudgeUpdatedIntegrationEvent eventMessage) 
            => await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, new[] {eventMessage.Judge});
        

        async Task IMessageHandler.HandleAsync(object integrationEvent) => await HandleAsync((JudgeUpdatedIntegrationEvent)integrationEvent);
        
    }
}