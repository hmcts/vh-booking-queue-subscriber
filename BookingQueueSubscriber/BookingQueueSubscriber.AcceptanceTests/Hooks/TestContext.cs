using BookingQueueSubscriber.AcceptanceTests.Configuration;

namespace BookingQueueSubscriber.AcceptanceTests.Hooks
{
    public class TestContext
    {
        public Config Config { get; set; }
        public Tokens Tokens { get; set; }
    }
}
