using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace BookingQueueSubscriber.Common.Logging;

[ExcludeFromCodeCoverage]
public class CloudRoleNameInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = "vh-booking-queue-subscriber";
    }
}