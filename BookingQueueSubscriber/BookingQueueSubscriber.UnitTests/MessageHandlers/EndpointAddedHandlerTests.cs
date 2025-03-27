using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointAddedHandlerTests : MessageHandlerTestBase
    {
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
                    SipAddress = integrationEvent.Endpoint.Sip,
                    DisplayName = integrationEvent.Endpoint.DisplayName,
                    LinkedParticipantIds = new List<Guid>(),
                    Pin = integrationEvent.Endpoint.Pin,
                    CurrentRoom = new RoomResponse { Id = 1, Label = "Room Label", Locked = false  }
                }
            };

            VideoApiServiceMock.Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(_mockEndpointDetailsResponse);
        }

        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new EndpointAddedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.AddEndpointToConference(It.IsAny<Guid>(), It.IsAny<AddEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new EndpointAddedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.AddEndpointToConference(It.IsAny<Guid>(), It.Is<AddEndpointRequest>
            (
                request =>
                    request.DisplayName == integrationEvent.Endpoint.DisplayName &&
                    request.ParticipantsLinked == integrationEvent.Endpoint.ParticipantsLinked &&
                    request.SipAddress == integrationEvent.Endpoint.Sip &&
                    request.Pin == integrationEvent.Endpoint.Pin &&
                    request.ConferenceRole == ConferenceRole.Host
            )), Times.Once);

            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        private EndpointAddedIntegrationEvent GetIntegrationEvent()
        {
            return new EndpointAddedIntegrationEvent
            {
                HearingId = HearingId,
                Endpoint = new EndpointDto{DisplayName = "one", Sip = Guid.NewGuid().ToString(), Pin = "1234", Role = Services.MessageHandlers.Dtos.ConferenceRole.Host}
            };
        }
    }
}