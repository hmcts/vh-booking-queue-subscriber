namespace BookingQueueSubscriber.Common.Configuration
{
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
    }
}