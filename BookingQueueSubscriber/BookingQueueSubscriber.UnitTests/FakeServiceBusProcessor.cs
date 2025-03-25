using System.Threading;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.Wrappers;

namespace BookingQueueSubscriber.UnitTests;

public class FakeServiceBusProcessor : IServiceBusProcessor
{
    public event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;
    public event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;

    public Task StartProcessingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopProcessingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
    
    public async Task TriggerMessageAsync(ProcessMessageEventArgs args)
    {
        if (ProcessMessageAsync != null)
            await ProcessMessageAsync.Invoke(args);
    }

    public async Task TriggerErrorAsync(ProcessErrorEventArgs args)
    {
        if (ProcessErrorAsync != null)
            await ProcessErrorAsync.Invoke(args);
    }
}