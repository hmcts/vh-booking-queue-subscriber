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
            var conferenceId = Guid.NewGuid();

            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When($"http://video-web/internalevent/HearingDateTimeChanged")
                .WithQueryString("conferenceId", conferenceId.ToString())
                .Respond(HttpStatusCode.NoContent);
            var service = CreateVideoWebService(mockHttp);

            // Act
            await service.PushHearingDateTimeChangedMessage(conferenceId);
            
            // Assert
            mockHttp.GetMatchCount(request).Should().Be(1);
        }
    }
}
