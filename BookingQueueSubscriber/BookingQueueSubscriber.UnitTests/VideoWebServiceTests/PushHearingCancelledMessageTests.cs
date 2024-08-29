using System.Net;
using RichardSzalay.MockHttp;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushHearingCancelledMessageTests : VideoWebServiceTests
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
            var service = CreateVideoWebService(mockHttp);

            // Act
            await service.PushHearingCancelledMessage(hearingId);
            
            // Assert
            mockHttp.GetMatchCount(request).Should().Be(1);
        }
    }
}
