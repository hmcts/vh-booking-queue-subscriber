namespace BookingQueueSubscriber.Common.Configuration
{
    public class HearingServicesConfiguration
    {
        public string VideoApiUrl { get; set; } = "https://localhost";
        public bool EnableVideoApiStub { get; set; }
    }
}