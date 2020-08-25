using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointRemovedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new EndpointRemovedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.RemoveEndpointFromConference(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new EndpointRemovedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.RemoveEndpointFromConference(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        private EndpointRemovedIntegrationEvent GetIntegrationEvent()
        {
            return new EndpointRemovedIntegrationEvent
            {
                HearingId = HearingId,
                EndpointId = Guid.NewGuid()
            };
        } 
    }
}