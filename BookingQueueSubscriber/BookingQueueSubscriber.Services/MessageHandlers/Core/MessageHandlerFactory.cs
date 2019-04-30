using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Get(MessageType messageType);
    }
    
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public MessageHandlerFactory(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }
        
        public IMessageHandler Get(MessageType messageType)
        {
            var eventHandler = _messageHandlers.SingleOrDefault(x => x.MessageType == messageType);
            if (eventHandler == null)
                throw new ArgumentOutOfRangeException(nameof(messageType),
                    $"MessageHandler cannot be found for messageType: {messageType.ToString()}");
            return eventHandler;
        }
    }
}