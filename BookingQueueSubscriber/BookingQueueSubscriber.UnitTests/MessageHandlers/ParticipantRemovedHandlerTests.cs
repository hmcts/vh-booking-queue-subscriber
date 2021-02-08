using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantRemovedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<ParticipantRemovedHandler>> _loggerMock =
            new Mock<ILogger<ParticipantRemovedHandler>>();
        
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantRemovedHandler(VideoApiServiceMock.Object, _loggerMock.Object);

            var integrationEvent = new ParticipantRemovedIntegrationEvent { HearingId = HearingId, ParticipantId = ParticipantId};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.RemoveParticipantFromConference(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler =
                (IMessageHandler) new ParticipantRemovedHandler(VideoApiServiceMock.Object, _loggerMock.Object);

            var integrationEvent = new ParticipantRemovedIntegrationEvent { HearingId = HearingId, ParticipantId = ParticipantId };
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.RemoveParticipantFromConference(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

    }
}