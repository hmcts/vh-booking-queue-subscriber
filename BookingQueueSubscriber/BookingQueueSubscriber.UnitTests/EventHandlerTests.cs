using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class EventHandlerTests
    {
        [Test]
        public void should_pass_dummytest()
        {
            var message = "{'eventType': 'HearingIsReadyForVideoIntegrationEvent'}";
            BookingQueueSubscriberFunction.Run(message, new LoggerFake());
            true.Should().BeTrue();
        }
    }
}
