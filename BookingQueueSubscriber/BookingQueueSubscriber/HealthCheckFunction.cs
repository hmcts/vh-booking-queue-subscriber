using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookingQueueSubscriber
{
    public class HealthCheckFunction
    {
        
        private readonly HealthCheckService _healthCheck;

        public HealthCheckFunction(HealthCheckService healthCheck)
        {
            _healthCheck = healthCheck;
        }

        [FunctionName("HealthCheck")]
        public async Task<IActionResult> HealthCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/liveness")]
            HttpRequest req, ILogger log)
        {
            var report = await _healthCheck.CheckHealthAsync();
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
