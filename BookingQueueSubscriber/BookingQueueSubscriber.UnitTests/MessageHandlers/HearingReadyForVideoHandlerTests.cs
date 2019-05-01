using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using FizzWare.NBuilder;
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
            var dto = CreateHearingDto();
            var message = new BookingsMessage
            {
                EventType = MessageType.HearingIsReadyForVideo,
                Message = dto
            };
            await messageHandler.HandleAsync(message);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
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
        
        private static HearingDto CreateHearingDto()
        {
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build();
            var dto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Civil Money Claims",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                Participants = participants
            };
            return dto;
        }
    }
}