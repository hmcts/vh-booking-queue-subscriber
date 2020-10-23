using BookingQueueSubscriber.Common.Configuration;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public ServicesConfiguration Services { get; set; }
        public string UsernameStem { get; set; }
    }
}
