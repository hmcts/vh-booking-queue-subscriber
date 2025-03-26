using System.Threading;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.HostedServices;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Wrappers;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests.HostedServices.ServiceBusListenerTests;

public class ServiceBusListenerTests
{
    private Mock<IMessageHandlerFactory> _mockHandlerFactory;
    private FakeServiceBusProcessor _processor;
    private Mock<ILogger<ServiceBusListener>> _mockLogger;
    private ServiceBusListener _serviceBusListener;
    private bool _stopProcessor = true;

    [SetUp]
    public void SetUp()
    {
        _mockHandlerFactory = new Mock<IMessageHandlerFactory>();
        _processor = new FakeServiceBusProcessor();
        _mockLogger = new Mock<ILogger<ServiceBusListener>>();

        _serviceBusListener = new ServiceBusListener(_mockHandlerFactory.Object, _processor, _mockLogger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        if (_stopProcessor)
            _serviceBusListener.StopAsync(CancellationToken.None).Wait();
        _stopProcessor = true;
    }
    
    [Test]
    public async Task StopAsync_StopsProcessor()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        
        await _serviceBusListener.StartAsync(cts.Token);

        // Act
        await _serviceBusListener.StopAsync(cts.Token);

        // Assert
        _processor.IsRunning.Should().BeFalse();

        _stopProcessor = false; // To avoid stopping it again as part of the tear down
    }
    
    [Test]
    public async Task ExecuteAsync_StartsProcessor()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        await _serviceBusListener.StartAsync(cts.Token);

        // Assert
        _processor.IsRunning.Should().BeTrue();
    }

    [Test]
    public async Task HandleError_IsCalledWhenExceptionThrownDuringProcessing()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var mockProcessor = new Mock<IServiceBusProcessorWrapper>();
        _serviceBusListener = new ServiceBusListener(_mockHandlerFactory.Object, mockProcessor.Object, _mockLogger.Object);

        Func<ProcessErrorEventArgs, Task> errorHandler = _ => Task.CompletedTask;
        mockProcessor
            .SetupAdd(p => p.ProcessErrorAsync += It.IsAny<Func<ProcessErrorEventArgs, Task>>())
            .Callback<Func<ProcessErrorEventArgs, Task>>(h => errorHandler = h);

        await _serviceBusListener.StartAsync(CancellationToken.None);

        // Act
        var processErrorEventArgs = new ProcessErrorEventArgs(
            exception,
            ServiceBusErrorSource.Receive,
            "test-namespace",
            "test-entity",
            CancellationToken.None
        );

        await errorHandler!(processErrorEventArgs);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error processing message")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
}