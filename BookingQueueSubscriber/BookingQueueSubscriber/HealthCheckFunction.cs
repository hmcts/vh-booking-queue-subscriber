using BookingQueueSubscriber.Contract.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using VideoApi.Client;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BookingQueueSubscriber
{
    public class HealthCheckFunction
    {
        [FunctionName("HealthCheck")]
        public async Task<IActionResult> HealthCheck(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req, ILogger log, [Inject] IVideoApiClient videoApiClient)
        {
            var response = new HealthCheckResponse
            {
                VideoApiHealth = { Successful = true },
                AppVersion = GetApplicationVersion()
            };

            try
            {
                await videoApiClient.GetExpiredOpenConferencesAsync();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unable to retrieve expired open conferences");
                response.VideoApiHealth = HandleVideoApiCallException(ex);
            }

            return new OkObjectResult(response);
        }

        private HealthCheck HandleVideoApiCallException(Exception ex)
        {
            var isApiException = ex is VideoApiException;
            var healthCheck = new HealthCheck { Successful = true };
            if (isApiException && ((VideoApiException)ex).StatusCode != (int)HttpStatusCode.InternalServerError)
            {
                return healthCheck;
            }

            healthCheck.Successful = false;
            healthCheck.ErrorMessage = ex.Message;
            healthCheck.Data = ex.Data;

            return healthCheck;
        }
        private ApplicationVersion GetApplicationVersion()
        {
            var applicationVersion = new ApplicationVersion();
            applicationVersion.FileVersion = GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
            applicationVersion.InformationVersion = GetExecutingAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
            return applicationVersion;
        }

        private string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}
