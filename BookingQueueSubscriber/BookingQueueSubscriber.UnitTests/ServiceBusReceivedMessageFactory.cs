using System.Reflection;
using Azure.Messaging.ServiceBus;

namespace BookingQueueSubscriber.UnitTests;

public static class ServiceBusReceivedMessageFactory
{
    public static ServiceBusReceivedMessage CreateFakeMessage(byte[] body)
    {
        var messageType = typeof(ServiceBusReceivedMessage);
        var constructor = messageType.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, 
            null,
            [typeof(ReadOnlyMemory<byte>)], 
            null
        );

        if (constructor == null)
            throw new InvalidOperationException("Cannot find the constructor for ServiceBusReceivedMessage.");

        return (ServiceBusReceivedMessage)constructor.Invoke([new ReadOnlyMemory<byte>(body)]);
    }
}