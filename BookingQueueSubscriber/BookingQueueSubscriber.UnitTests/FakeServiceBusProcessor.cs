using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.Wrappers;

namespace BookingQueueSubscriber.UnitTests;

[SuppressMessage("Usage", "CS0067", Justification = "Events are used only by unit tests")]
public class FakeServiceBusProcessor : IServiceBusProcessorWrapper
{
    #pragma warning disable CS0067
    public event Func<ProcessMessageEventArgs, Task> ProcessMessageAsync;
    public event Func<ProcessErrorEventArgs, Task> ProcessErrorAsync;
    #pragma warning disable CS0067
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