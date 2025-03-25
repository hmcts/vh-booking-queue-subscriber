using Azure.Messaging.ServiceBus;

namespace BookingQueueSubscriber;

public interface IServiceBusProcessorWrapper : IAsyncDisposable
{
    Task StartProcessingAsync(CancellationToken cancellationToken);
    Task StopProcessingAsync(CancellationToken cancellationToken);
    void AddMessageHandler(Func<ProcessMessageEventArgs, Task> handler);
    void AddErrorHandler(Func<ProcessErrorEventArgs, Task> handler);
    void RemoveMessageHandler(Func<ProcessMessageEventArgs, Task> handler);
    void RemoveErrorHandler(Func<ProcessErrorEventArgs, Task> handler);
}

public class ServiceBusProcessorWrapper(ServiceBusProcessor serviceBusProcessor) : IServiceBusProcessorWrapper
{
    private readonly ServiceBusProcessor _serviceBusProcessor = serviceBusProcessor ?? throw new ArgumentNullException(nameof(serviceBusProcessor));

    public Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        return _serviceBusProcessor.StartProcessingAsync(cancellationToken);
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken)
    {
        return _serviceBusProcessor.StopProcessingAsync(cancellationToken);
    }

    public void AddMessageHandler(Func<ProcessMessageEventArgs, Task> handler)
    {
        _serviceBusProcessor.ProcessMessageAsync += handler;
    }

    public void AddErrorHandler(Func<ProcessErrorEventArgs, Task> handler)
    {
        _serviceBusProcessor.ProcessErrorAsync += handler;
    }
    
    public void RemoveMessageHandler(Func<ProcessMessageEventArgs, Task> handler)
    {
        _serviceBusProcessor.ProcessMessageAsync -= handler;
    }

    public void RemoveErrorHandler(Func<ProcessErrorEventArgs, Task> handler)
    {
        _serviceBusProcessor.ProcessErrorAsync -= handler;
    }
    
    public ValueTask DisposeAsync()
    {
        return _serviceBusProcessor.DisposeAsync();
    }
}