using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointAddedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new EndpointAddedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.AddEndpointToConference(It.IsAny<Guid>(), It.IsAny<AddEndpointRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new EndpointAddedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.AddEndpointToConference(It.IsAny<Guid>(), It.Is<AddEndpointRequest>
            (
                request => request.DisplayName == integrationEvent.Endpoint.DisplayName &&
                           request.SipAddress == integrationEvent.Endpoint.Sip &&
                           request.Pin == integrationEvent.Endpoint.Pin && 
                           request.DefenceAdvocate == integrationEvent.Endpoint.DefenceAdvocate
            )), Times.Once);
        }

        private EndpointAddedIntegrationEvent GetIntegrationEvent()
        {
            return new EndpointAddedIntegrationEvent
            {
                HearingId = HearingId,
                Endpoint = new EndpointDto{DisplayName = "one", Sip = Guid.NewGuid().ToString(), Pin = "1234"}
            };
        }
    }
}