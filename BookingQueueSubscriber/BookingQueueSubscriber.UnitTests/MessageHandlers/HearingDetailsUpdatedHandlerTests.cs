using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingDetailsUpdatedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new HearingDetailsUpdatedHandler(VideoApiServiceMock.Object);

            var integrationEvent = new HearingDetailsUpdatedIntegrationEvent()
            {
                Hearing = new HearingDto
                {
                    CaseName = "CaseName",
                    CaseNumber = "caseNO",
                    CaseType = "caseType",
                    HearingId = Guid.NewGuid(),
                    ScheduledDateTime = DateTime.Now,
                    ScheduledDuration = 30
                }
            };
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateConferenceAsync(It.IsAny<UpdateConferenceRequest>()), Times.Once);
        }
    }
}