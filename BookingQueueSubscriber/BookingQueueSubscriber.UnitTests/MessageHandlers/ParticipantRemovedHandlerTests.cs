using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantRemovedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantRemovedHandler(VideoApiServiceMock.Object);

            var integrationEvent = new ParticipantRemovedIntegrationEvent { HearingId = HearingId, ParticipantId = ParticipantId};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.RemoveParticipantFromConference(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}