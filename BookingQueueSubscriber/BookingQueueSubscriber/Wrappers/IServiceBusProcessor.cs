using Azure.Messaging.ServiceBus;

namespace BookingQueueSubscriber.Wrappers;

public interface IServiceBusProcessor
{
    event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;
    event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;

    Task StartProcessingAsync(CancellationToken cancellationToken);
    Task StopProcessingAsync(CancellationToken cancellationToken);
    Task DisposeAsync();
}