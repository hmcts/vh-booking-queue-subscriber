using System.Collections;

namespace BookingQueueSubscriber.Contract.Responses
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            VideoApiHealth = new HealthCheck();
            AppVersion = new ApplicationVersion();
        }

        public HealthCheck VideoApiHealth { get; set; }
        public ApplicationVersion AppVersion { get; set; }

    }

    public class HealthCheck
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary Data { get; set; }
    }

    public class ApplicationVersion
    {
        public string FileVersion { get; set; }
        public string InformationVersion { get; set; }
    }
}
