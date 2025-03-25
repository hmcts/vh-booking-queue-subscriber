using System.Threading;
using BookingQueueSubscriber.HostedServices;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests.HostedServices.ServiceBusListenerTests;

public class ServiceBusListenerTests
{
    private Mock<IMessageHandlerFactory> _mockHandlerFactory;
    private Mock<IServiceBusProcessorWrapper> _mockProcessor;
    private Mock<ILogger<ServiceBusListener>> _mockLogger;
    private ServiceBusListener _listener;

    [SetUp]
    public void SetUp()
    {
        _mockHandlerFactory = new Mock<IMessageHandlerFactory>();
        _mockProcessor = new Mock<IServiceBusProcessorWrapper>();
        _mockLogger = new Mock<ILogger<ServiceBusListener>>();

        _listener = new ServiceBusListener(_mockHandlerFactory.Object, _mockProcessor.Object, _mockLogger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _listener.StopAsync(CancellationToken.None).Wait();
    }
    
    [Test]
    public async Task StopAsync_StopsAndDisposesProcessor()
    {
        // Arrange
        _mockProcessor
            .Setup(p => p.StopProcessingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    
        _mockProcessor
            .Setup(p => p.DisposeAsync())
            .Returns(ValueTask.CompletedTask);

        // Act
        await _listener.StopAsync(CancellationToken.None);

        // Assert
        _mockProcessor.Verify(p => p.StopProcessingAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockProcessor.Verify(p => p.DisposeAsync(), Times.Once);
    }
    
    [Test]
    public async Task ExecuteAsync_StartsServiceBusProcessor()
    {
        // Arrange
        _mockProcessor
            .Setup(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource();

        // Act
        await _listener.StartAsync(cts.Token);

        // Give some time to start before cancellation
        await Task.Delay(100, cts.Token);
        await cts.CancelAsync(); // Simulate service stop

        // Assert
        _mockProcessor.Verify(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}