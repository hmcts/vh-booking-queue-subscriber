using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace BookingQueueSubscriber.Common.Logging;

[ExcludeFromCodeCoverage]
public class RequestBodyInitializer : ITelemetryInitializer
{
    readonly IHttpContextAccessor httpContextAccessor;

    public RequestBodyInitializer(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is RequestTelemetry requestTelemetry &&
            IsMethodAllowed(httpContextAccessor.HttpContext.Request.Method))
        {
            const string jsonBody = "JsonBody";

            if (requestTelemetry.Properties.ContainsKey(jsonBody))
            {
                return;
            }

            //Allows re-usage of the stream
            httpContextAccessor.HttpContext.Request.EnableBuffering();

            var stream = new StreamReader(httpContextAccessor.HttpContext.Request.Body);
            var body = stream.ReadToEnd();

            //Reset the stream so data is not lost
            httpContextAccessor.HttpContext.Request.Body.Position = 0;
            requestTelemetry.Properties.Add(jsonBody, body);

        }
    }

    private bool IsMethodAllowed(string method)
    {
        return method == HttpMethods.Post || method == HttpMethods.Put || method == HttpMethods.Patch;
    }
}