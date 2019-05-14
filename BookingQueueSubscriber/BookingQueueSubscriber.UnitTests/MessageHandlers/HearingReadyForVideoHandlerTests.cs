using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
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
         
            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
        }

        private static HearingIsReadyForVideoIntegrationEvent CreateEvent()
        {
            var hearingDto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Civil Money Claims",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow
            };
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build().ToList();
            
            var message = new HearingIsReadyForVideoIntegrationEvent
            {
                Hearing = hearingDto,
                Participants = participants
            };
            return message;
        }
    }
}