using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber.HostedServices;

// 📌 Health Check Hosted Service to expose HTTP endpoint
public class HealthCheckService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    private WebApplication? _app;

    public HealthCheckService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
    {
        _appLifetime = appLifetime;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();

        // Configure Kestrel
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8080);
        });

        builder.Services.AddSingleton(_serviceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>());

        _app = builder.Build();

        // Map the health check endpoint
        _app.MapHealthChecks("/health/liveness");

        _appLifetime.ApplicationStopping.Register(() => _app?.DisposeAsync());

        // Start the app
        await _app.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_app != null)
        {
            await _app.StopAsync(cancellationToken);
        }
    }
}