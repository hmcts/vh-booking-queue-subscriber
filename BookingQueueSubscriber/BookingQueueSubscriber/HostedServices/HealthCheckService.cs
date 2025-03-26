using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BookingQueueSubscriber.HostedServices;

public class HealthCheckService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
    : IHostedService
{
    public WebApplication? App { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8090);
        });

        builder.Services.AddSingleton(serviceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>());

        App = builder.Build();
        App.MapHealthChecks("/health/liveness");

        appLifetime.ApplicationStopping.Register(() =>
        {
            if (App != null)
            {
                _ = Task.Run(async () => await App.DisposeAsync(), cancellationToken);
            }
        });
        
        await App.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (App != null)
        {
            await App.StopAsync(cancellationToken);
        }
    }
}