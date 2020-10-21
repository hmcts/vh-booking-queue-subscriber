namespace BookingQueueSubscriber.Common.Configuration
{
    public class ServicesConfiguration
    {
        public string BookingsApiUrl { get; set; }
        public string VideoApiUrl { get; set; } = "https://localhost";
        public bool EnableVideoApiStub { get; set; }
    }
}