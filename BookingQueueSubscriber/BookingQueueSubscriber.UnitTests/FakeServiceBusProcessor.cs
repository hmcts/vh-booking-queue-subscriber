using System.Threading;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.Wrappers;

namespace BookingQueueSubscriber.UnitTests;

public class FakeServiceBusProcessor : IServiceBusProcessorWrapper
{
    public event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;
    public event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;
    public bool IsRunning { get; private set; }

    public Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        return Task.CompletedTask;
    }

    public Task StopProcessingAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}