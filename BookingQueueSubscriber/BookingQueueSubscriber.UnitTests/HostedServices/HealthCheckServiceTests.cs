using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using HealthCheckService = BookingQueueSubscriber.HostedServices.HealthCheckService;

namespace BookingQueueSubscriber.UnitTests.HostedServices;

public class HealthCheckServiceTests
{
    private IServiceProvider _serviceProvider;
    private HealthCheckService _healthCheckService;

    [SetUp]
    public void Setup()
    {
        var serviceCollection = new ServiceCollection();
        var appLifetimeMock = new Mock<IHostApplicationLifetime>().Object;
        
        serviceCollection.AddLogging();
        serviceCollection.AddHealthChecks();
        serviceCollection.Configure<HealthCheckServiceOptions>(_ => { });
        serviceCollection.AddSingleton<HealthCheckService>();
        serviceCollection.AddSingleton(appLifetimeMock);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        _healthCheckService = _serviceProvider.GetRequiredService<HealthCheckService>();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _healthCheckService.StopAsync(CancellationToken.None);
    }
    
    [Test]
    public async Task StartAsync_ShouldConfigureAndStartApplication()
    {
        // Arrange & Act
        await _healthCheckService.StartAsync(CancellationToken.None);

        // Assert
        _healthCheckService.App.Should().NotBeNull();
    }

    [Test]
    public async Task StopAsync_ShouldStopApplication()
    {
        // Arrange
        await _healthCheckService.StartAsync(CancellationToken.None);

        // Act
        await _healthCheckService.StopAsync(CancellationToken.None);

        // Assert
        Assert.DoesNotThrowAsync(async () => await _healthCheckService.StopAsync(CancellationToken.None));
    }

    [Test]
    public Task StopAsync_ShouldNotThrowWhenCalledBeforeStart()
    {
        Assert.DoesNotThrowAsync(async () => await _healthCheckService.StopAsync(CancellationToken.None));
        return Task.CompletedTask;
    }
}