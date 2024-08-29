using System.Net;
using RichardSzalay.MockHttp;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushHearingDateTimeChangedMessageTests : VideoWebServiceTests
    {
        [Test]
        public async Task should_push_message()
        {
            // Arrange
            var hearingId = Guid.NewGuid();

            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When($"http://video-web/internalevent/HearingDateTimeChanged")
                .WithQueryString("hearingId", hearingId.ToString())
                .Respond(HttpStatusCode.NoContent);
            var service = CreateVideoWebService(mockHttp);

            // Act
            await service.PushHearingDateTimeChangedMessage(hearingId);
            
            // Assert
            mockHttp.GetMatchCount(request).Should().Be(1);
        }
    }
}
