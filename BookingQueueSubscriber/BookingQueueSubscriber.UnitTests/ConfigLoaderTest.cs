using BookingQueueSubscriber.Common.Configuration;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class ConfigLoaderTest
    {
        [Test]
        public void should_get_configuration_object()
        {
            var configLoader = new ConfigLoader();
            configLoader.Configuration.Should().NotBeNull();
        }
    }
}