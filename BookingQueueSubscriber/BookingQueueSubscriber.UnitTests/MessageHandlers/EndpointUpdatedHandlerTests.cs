using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Responses;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointUpdatedHandlerTests : MessageHandlerTestBase
    {
        protected Mock<ILogger<EndpointUpdatedHandler>> logger;
        private ICollection<EndpointResponse> _mockEndpointDetailsResponse;

        [SetUp]
        public new void Setup()
        {
            var integrationEvent = GetIntegrationEvent();

            _mockEndpointDetailsResponse = new List<EndpointResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SipAddress = integrationEvent.Sip,
                    DisplayName = integrationEvent.DisplayName,
                    LinkedParticipantIds = new List<Guid>(),
                    Pin = "Pin",
                    CurrentRoom = new RoomResponse { Id = 1, Label = "Room Label", Locked = false  },
                    ConferenceRole = ConferenceRole.Host
                }
            };

            VideoApiServiceMock.Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(_mockEndpointDetailsResponse);
        }

        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, logger.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            var messageHandler = (IMessageHandler) new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, logger.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(),
                It.Is<UpdateEndpointRequest>
                (
                    request => request.DisplayName == integrationEvent.DisplayName &&
                               request.ParticipantsLinked.SequenceEqual(integrationEvent.ParticipantsLinked) 
                )), Times.Once);

            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(), 
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        private EndpointUpdatedIntegrationEvent GetIntegrationEvent()
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = HearingId,
                Sip = Guid.NewGuid().ToString(),
                DisplayName = "two",
                ParticipantsLinked = new List<string>{"test@hmcts.net"} 
            };
        }
    }
}