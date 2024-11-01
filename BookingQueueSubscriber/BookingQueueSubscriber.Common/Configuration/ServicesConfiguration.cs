using System.Diagnostics.CodeAnalysis;

namespace BookingQueueSubscriber.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ServicesConfiguration
    {
        public string BookingsApiUrl { get; set; }
        public string BookingsApiResourceId { get; set; }
        public string VideoApiResourceId { get; set; }
        public string VideoApiUrl { get; set; }
        public bool EnableVideoApiStub { get; set; }
        public string VideoWebResourceId { get; set; }
        public string VideoWebUrl { get; set; }
        public string InternalEventSecret { get; set; }
        public string UserApiResourceId { get; set; }
        public string UserApiUrl { get; set; } = "https://localhost:5200/";
        public string NotificationApiResourceId { get; set; }
        public string NotificationApiUrl { get; set; } = "https://localhost:59390/";
    }
}