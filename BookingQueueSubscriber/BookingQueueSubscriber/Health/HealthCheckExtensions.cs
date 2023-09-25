using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookingQueueSubscriber.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddVhHealthChecks(this IServiceCollection services)
    {
        var container = services.BuildServiceProvider();
        var servicesConfiguration = container.GetService<IOptions<ServicesConfiguration>>().Value;
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddUrlGroup(
                new Uri(
                    new Uri(servicesConfiguration.VideoApiUrl),
                    "/health/liveness"),
                name: "Video API",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] {"services"})
            .AddUrlGroup(
                new Uri(
                    new Uri(servicesConfiguration.BookingsApiUrl),
                    "/health/liveness"),
                name: "Bookings API",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] {"services"})
            .AddUrlGroup(
                new Uri(
                    new Uri(servicesConfiguration.UserApiUrl),
                    "/health/liveness"),
                name: "User API",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] {"services"});
        return services;
    }
}
