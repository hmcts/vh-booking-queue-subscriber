using System;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class EventHandlerTests
    {
        [Test]
        public void should_pass_dummytest()
        {
            true.Should().BeTrue();
        }
    }
}
