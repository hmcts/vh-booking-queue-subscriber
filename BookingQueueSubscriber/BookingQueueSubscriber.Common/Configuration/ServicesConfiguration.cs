namespace BookingQueueSubscriber.Common.Configuration
{
    public class ServicesConfiguration
    {
        public string BookingsApiUrl { get; set; }
        public string VideoApiResourceId { get; set; }
        public string VideoApiUrl { get; set; }
        public bool EnableVideoApiStub { get; set; }
    }
}