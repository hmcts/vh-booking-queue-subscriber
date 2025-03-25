using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber.HostedServices;

public class HealthCheckService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
    : IHostedService
{
    private WebApplication? _app;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8090);
        });

        builder.Services.AddSingleton(serviceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>());

        _app = builder.Build();
        _app.MapHealthChecks("/health/liveness");

        appLifetime.ApplicationStopping.Register(() => _app?.DisposeAsync());
        
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