using System.Net;
using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushHearingCancelledMessageTests
    {
        [Test]
        public async Task should_push_message()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When($"http://video-web/internalevent/HearingCancelled")
                .WithQueryString("hearingId", hearingId.ToString())
                .Respond(HttpStatusCode.NoContent);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://video-web");
            var service = new VideoWebService(httpClient, new Mock<ILogger<VideoWebService>>().Object);

            // Act
            await service.PushHearingCancelledMessage(hearingId);
            
            // Assert
            mockHttp.GetMatchCount(request).Should().Be(1);
        }
    }
}
