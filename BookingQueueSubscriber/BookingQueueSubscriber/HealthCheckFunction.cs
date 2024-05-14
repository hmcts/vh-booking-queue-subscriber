using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookingQueueSubscriber
{
    public class HealthCheckFunction(HealthCheckService healthCheck)
    {
        [Function("HealthCheck")]
        public async Task<IActionResult> HealthCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/liveness")]
            HttpRequest req)
        {
            var report = await healthCheck.CheckHealthAsync();
            var healthCheckResponse = new
            {
                status = report.Status.ToString(),
                details = report.Entries.Select(e => new
                {
                    key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                    error = e.Value.Exception?.Message
                })
            };

            var statusCode = report.Status == HealthStatus.Healthy
                ? (int) HttpStatusCode.OK
                : (int) HttpStatusCode.ServiceUnavailable;

            return new ObjectResult(healthCheckResponse)
            {
                StatusCode = statusCode
            };
        }
    }
}
