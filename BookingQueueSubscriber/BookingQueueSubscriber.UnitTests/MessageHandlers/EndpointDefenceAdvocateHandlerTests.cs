using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using VideoApi.Contract.Requests;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Responses;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointDefenceAdvocateHandlerTests : MessageHandlerTestBase
    {
        protected Mock<ILogger<EndpointUpdatedHandler>> logger;
        private ICollection<EndpointResponse> _mockEndpointDetailsResponse;

        [SetUp]
        public new void Setup()
        {
            var integrationEvent = GetIntegrationEventValid();

            _mockEndpointDetailsResponse = new List<EndpointResponse>
            {
                new EndpointResponse
                {
                    Id = Guid.NewGuid(),
                    SipAddress = integrationEvent.Sip,
                    DisplayName = integrationEvent.DisplayName,
                    DefenceAdvocate = integrationEvent.DefenceAdvocate,
                    Pin = "Pin",
                    CurrentRoom = new RoomResponse { Id = 1, Label = "Room Label", Locked = false  }
                }
            };

            VideoApiServiceMock.Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(_mockEndpointDetailsResponse);
        }

        [Test]
        public async Task should_log_error_when_conference_is_null()
        {
            ConferenceDetailsResponse conference = null; 
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(conference);

            logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, logger.Object);

            var integrationEvent = GetIntegrationEventValid();
            await messageHandler.HandleAsync(integrationEvent);

            logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Unable to find conference by hearing id")),
                It.Is<Exception>(x => x == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Never);
        }

        [Test]
        public async Task should_log_error_when_retry_limit_is_reached()
        {
            logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, logger.Object);

            var integrationEvent = GetIntegrationEventInvalid();
            await messageHandler.HandleAsync(integrationEvent);

            logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Unable to find defence advocate email by hearing id")),
                It.Is<Exception>(x => x == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        [Test]
        public async Task should_not_log_error_when_defence_advocate_is_found()
        {
            logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, logger.Object);

            var integrationEvent = GetIntegrationEventValid();
            await messageHandler.HandleAsync(integrationEvent);

            logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Unable to find defence advocate email by hearing id")),
                It.Is<Exception>(x => x == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }

        private EndpointUpdatedIntegrationEvent GetIntegrationEventValid()
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = HearingId,
                Sip = Guid.NewGuid().ToString(),
                DisplayName = "two",
                DefenceAdvocate = "test@hmcts.net"
            };
        }

        private EndpointUpdatedIntegrationEvent GetIntegrationEventInvalid()
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = HearingId,
                Sip = Guid.NewGuid().ToString(),
                DisplayName = "two",
                DefenceAdvocate = "test1@hmcts.net"
            };
        }
    }
}