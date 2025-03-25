using Azure.Messaging.ServiceBus;

namespace BookingQueueSubscriber.Wrappers;

public interface IServiceBusReceivedMessage
{
    BinaryData Body { get; }
}

public class ServiceBusReceivedMessageWrapper(ServiceBusReceivedMessage message) : IServiceBusReceivedMessage
{
    public BinaryData Body => message.Body;
}