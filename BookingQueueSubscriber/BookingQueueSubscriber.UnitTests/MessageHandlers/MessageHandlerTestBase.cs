using BookingQueueSubscriber.Services;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public abstract class MessageHandlerTestBase
    {
        protected Mock<IVideoApiService> VideoApiServiceMock { get; set; }

        [SetUp]
        public void Setup()
        {
            VideoApiServiceMock = new Mock<IVideoApiService>();
        }
    }
}