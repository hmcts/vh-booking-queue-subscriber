using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingReadyForVideoHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new HearingReadyForVideoHandler(VideoApiServiceMock.Object);
            var request = new BookNewConferenceRequest();
            var message = new BookingsMessage
            {
                EventType = MessageType.HearingIsReadyForVideo,
                Message = request
            };
            await messageHandler.HandleAsync(message);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(request), Times.Once);
        }
        
        [Test]
        public void should_not_call_video_api_when_request_is_invalid()
        {
            var messageHandler = new HearingReadyForVideoHandler(VideoApiServiceMock.Object);
            var request = new AddParticipantsToConferenceRequest();
            var message = new BookingsMessage
            {
                EventType = MessageType.HearingIsReadyForVideo,
                Message = request
            };
            Assert.ThrowsAsync<InvalidCastException>(() => messageHandler.HandleAsync(message));
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Never);
        }
    }
}