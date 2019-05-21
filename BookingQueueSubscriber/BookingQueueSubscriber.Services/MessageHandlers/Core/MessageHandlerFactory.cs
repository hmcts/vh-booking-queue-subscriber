using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Get(IntegrationEventType integrationEventType);
        IEnumerable<IMessageHandler> MessageHandlers { get; }
    }
    
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public MessageHandlerFactory(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }
        
        public IMessageHandler Get(IntegrationEventType integrationEventType)
        {
            var eventHandler = _messageHandlers.SingleOrDefault(x => x.IntegrationEventType == integrationEventType);
            if (eventHandler == null)
                throw new ArgumentOutOfRangeException(nameof(integrationEventType),
                    $"MessageHandler cannot be found for messageType: {integrationEventType.ToString()}");
            return eventHandler;
        }

        public IEnumerable<IMessageHandler> MessageHandlers => _messageHandlers;
    }
}