using Azure.Messaging.ServiceBus;

namespace BookingQueueSubscriber.Wrappers;

public interface IServiceBusProcessorWrapper
{
    event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;
    event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;

    Task StartProcessingAsync(CancellationToken cancellationToken);
    Task StopProcessingAsync(CancellationToken cancellationToken);
    Task DisposeAsync();
}

public class ServiceBusProcessorWrapper(IServiceBusProcessor processor) : IServiceBusProcessorWrapper
{
    public event Func<ProcessMessageEventArgs, Task>? ProcessMessageAsync
    {
        add => processor.ProcessMessageAsync += value;
        remove => processor.ProcessMessageAsync -= value;
    }

    public event Func<ProcessErrorEventArgs, Task>? ProcessErrorAsync
    {
        add => processor.ProcessErrorAsync += value;
        remove => processor.ProcessErrorAsync -= value;
    }

    public Task StartProcessingAsync(CancellationToken cancellationToken) => processor.StartProcessingAsync(cancellationToken);
    public Task StopProcessingAsync(CancellationToken cancellationToken) => processor.StopProcessingAsync(cancellationToken);
    public async Task DisposeAsync() => await processor.DisposeAsync();
}