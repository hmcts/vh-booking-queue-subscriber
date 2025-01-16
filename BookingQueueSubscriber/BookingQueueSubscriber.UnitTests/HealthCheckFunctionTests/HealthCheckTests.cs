using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests.HealthCheckFunctionTests;

public class HealthCheckTests
{
    private HealthCheckFunction _sut;
    private Mock<HealthCheckService> _healthCheckServiceMock;

    [SetUp]
    public void Setup()
    {
        _healthCheckServiceMock = new Mock<HealthCheckService>();
        _sut = new HealthCheckFunction(_healthCheckServiceMock.Object);
    }
    
    [Test]
    public async Task Should_return_200_when_health_check_is_healthy()
    {
        // Arrange
        var healthReport = new HealthReport(new Dictionary<string, HealthReportEntry>
        {
            {"test", new HealthReportEntry(HealthStatus.Healthy, "test", TimeSpan.Zero, null, null)}
        }, TimeSpan.Zero);
        _healthCheckServiceMock.Setup(x=> x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration,bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);
        
        // Act
        var result = await _sut.HealthCheck(null, Mock.Of<ILogger>());
        
        // Assert
        var okResult = result as ObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That((int)HttpStatusCode.OK, Is.EqualTo(okResult.StatusCode));
    }
    
    [Test]
    public async Task Should_return_503_when_health_check_is_unhealthy()
    {
        // Arrange
        var healthReport = new HealthReport(new Dictionary<string, HealthReportEntry>
        {
            {"test", new HealthReportEntry(HealthStatus.Unhealthy, "test", TimeSpan.Zero, new Exception("Not Working Right Now"), null)}
        }, TimeSpan.Zero);
        _healthCheckServiceMock.Setup(x=> x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration,bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthReport);
        
        // Act
        var result = await _sut.HealthCheck(null, Mock.Of<ILogger>());
        
        // Assert
        var serviceUnavailableResult = result as ObjectResult;
        Assert.That(serviceUnavailableResult, Is.Not.Null);
        Assert.That((int)HttpStatusCode.ServiceUnavailable, Is.EqualTo(serviceUnavailableResult.StatusCode));
    }
}