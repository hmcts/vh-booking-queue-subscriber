using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public abstract class VideoWebServiceTests
    {
        protected static VideoWebService CreateVideoWebService(MockHttpMessageHandler mockHttp)
        {
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://video-web");
            return new VideoWebService(httpClient, new Mock<ILogger<VideoWebService>>().Object);
        }
    }
}
