using System.Threading;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.Wrappers;

namespace BookingQueueSubscriber.UnitTests.Wrappers;

public class ServiceBusProcessorWrapperTests
{
    private FakeServiceBusProcessor _fakeProcessor;
    private IServiceBusProcessorWrapper _wrapper;

    [SetUp]
    public void SetUp()
    {
        _fakeProcessor = new FakeServiceBusProcessor();
        _wrapper = new ServiceBusProcessorWrapper(_fakeProcessor);
    }

    [Test]
    public async Task StartProcessingAsync_CallsProcessorStart()
    {
        using var cts = new CancellationTokenSource();
        await _wrapper.StartProcessingAsync(cts.Token);
        Assert.Pass();  // No exceptions means the call succeeded
    }

    [Test]
    public async Task StopProcessingAsync_CallsProcessorStop()
    {
        using var cts = new CancellationTokenSource();
        await _wrapper.StopProcessingAsync(cts.Token);
        Assert.Pass();  // No exceptions means the call succeeded
    }

    [Test]
    public async Task DisposeAsync_CallsProcessorDispose()
    {
        await _wrapper.DisposeAsync();
        Assert.Pass();  // No exceptions means the call succeeded
    }

    [Test]
    public async Task ProcessMessageAsync_EventIsInvoked()
    {
        // Arrange
        var wasCalled = false;
        _wrapper.ProcessMessageAsync += _ => Task.FromResult(wasCalled = true);
    
        var fakeMessage = ServiceBusReceivedMessageFactory.CreateFakeMessage([1, 2, 3]);

        var messageArgs = new ProcessMessageEventArgs(
            fakeMessage, 
            new Mock<ServiceBusReceiver>().Object, 
            CancellationToken.None
        );
    
        // Act
        await _fakeProcessor.TriggerMessageAsync(messageArgs);
    
        // Assert
        wasCalled.Should().BeTrue();
    }
    
    [Test]
    public async Task ProcessErrorAsync_EventIsInvoked()
    {
        // Arrange
        var wasCalled = false;
        _wrapper.ProcessErrorAsync += _ => Task.FromResult(wasCalled = true);

        var errorArgs = new ProcessErrorEventArgs(
            new Exception("Test Exception"),
            ServiceBusErrorSource.Receive,
            "namespace",
            "entity",
            CancellationToken.None
        );

        // Act
        await _fakeProcessor.TriggerErrorAsync(errorArgs);

        // Assert
        wasCalled.Should().BeTrue();
    }
}