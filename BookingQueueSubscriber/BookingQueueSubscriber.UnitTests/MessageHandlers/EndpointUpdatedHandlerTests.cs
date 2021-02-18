using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using VideoApi.Contract.Requests;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointUpdatedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new EndpointUpdatedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(),
                It.Is<UpdateEndpointRequest>
                (
                    request => request.DisplayName == integrationEvent.DisplayName &&
                               request.DefenceAdvocate == integrationEvent.DefenceAdvocate
                )), Times.Once);
        }

        private EndpointUpdatedIntegrationEvent GetIntegrationEvent()
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = HearingId,
                Sip = Guid.NewGuid().ToString(),
                DisplayName = "two",
                DefenceAdvocate = "test@hmcts.net"
            };
        }
    }
}