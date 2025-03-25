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
    private Mock<IServiceBusProcessorWrapper> _mockProcessor;
    private Mock<ILogger<ServiceBusListener>> _mockLogger;
    private ServiceBusListener _serviceBusListener;
    private bool _stopProcessor = true;

    [SetUp]
    public void SetUp()
    {
        _mockHandlerFactory = new Mock<IMessageHandlerFactory>();
        _mockProcessor = new Mock<IServiceBusProcessorWrapper>();
        _mockLogger = new Mock<ILogger<ServiceBusListener>>();

        _serviceBusListener = new ServiceBusListener(_mockHandlerFactory.Object, _mockProcessor.Object, _mockLogger.Object);
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
        _mockProcessor
            .Setup(m => m.StopProcessingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        using var cts = new CancellationTokenSource();
        
        await _serviceBusListener.StartAsync(cts.Token);

        // Act
        await _serviceBusListener.StopAsync(cts.Token);

        // Assert
        _mockProcessor.Verify(m => m.StopProcessingAsync(It.IsAny<CancellationToken>()), Times.Once);

        _stopProcessor = false; // To avoid stopping it again as part of the tear down
    }
    
    [Test]
    public async Task ExecuteAsync_StartsProcessor()
    {
        // Arrange
        _mockProcessor
            .Setup(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await _serviceBusListener.StartAsync(cts.Token);

        // Assert
        _mockProcessor.Verify(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleError_IsCalledWhenExceptionThrownDuringProcessing()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var mockProcessor = new Mock<IServiceBusProcessorWrapper>();
        _serviceBusListener = new ServiceBusListener(_mockHandlerFactory.Object, mockProcessor.Object, _mockLogger.Object);

        Func<ProcessErrorEventArgs, Task>? errorHandler = null;
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
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}