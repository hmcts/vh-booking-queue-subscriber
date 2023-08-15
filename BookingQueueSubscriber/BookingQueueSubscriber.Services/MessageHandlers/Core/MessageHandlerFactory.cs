namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Get<T>(T integrationEvent) where T: IIntegrationEvent;
    }

    public class MessageHandlerFactory : IMessageHandlerFactory 
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMessageHandler Get<T>(T integrationEvent) where T: IIntegrationEvent
        {
            var genericType = typeof(IMessageHandler<>).MakeGenericType(integrationEvent.GetType());
            var service = _serviceProvider.GetService(genericType);

            return (IMessageHandler) service;
        }
    }
}