using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Responses;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.Models;
using VideoApi.Contract.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class EndpointDefenceAdvocateHandlerTests : MessageHandlerTestBase
    {
        private Mock<ILogger<EndpointUpdatedHandler>> _logger;
        private Guid _hearingId = Guid.NewGuid();
        private Guid _conferenceId = Guid.NewGuid();
        private const string SipAddress = "SipAddress";
        private const string Endpoint = "JvsEndpointName";
        private readonly DefenceAdvocate _defenceAdvocate1 = new("rep1Username@email.com", "rep1ContactEmail@email.com");
        private readonly DefenceAdvocate _defenceAdvocate2 = new("rep2Username@email.com", "rep2ContactEmail@email.com");
        private readonly DefenceAdvocate _defenceAdvocate3 = new("rep3Username@email.com", "rep3ContactEmail@email.com");
        private EndpointUpdatedIntegrationEvent GetIntegrationEventValid(DefenceAdvocate defenceAdvocate)
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = _hearingId,
                Sip = SipAddress,
                DisplayName = Endpoint,
                DefenceAdvocate = defenceAdvocate?.ContactEmail
            };
        }
        private List<EndpointResponse> GetEndpointsForConference(DefenceAdvocate defenceAdvocate, EndpointState state = EndpointState.Connected)
        {
            return new List<EndpointResponse>
            {
                new()
                {
                    Id = _hearingId,
                    SipAddress = SipAddress,
                    DisplayName = "endpointName",
                    DefenceAdvocate = defenceAdvocate?.Username,
                    Status = state,
                    Pin = "Pin",
                    CurrentRoom = new RoomResponse { Id = 1, Label = "Room Label", Locked = false }
                }
            };
        }
        [SetUp]
        public new void Setup()
        {
            _logger = new Mock<ILogger<EndpointUpdatedHandler>>();
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Id = _conferenceId,
                    HearingId = _hearingId,
                    Participants = new List<ParticipantResponse>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = _defenceAdvocate1.Username,
                            ContactEmail = _defenceAdvocate1.ContactEmail,
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = _defenceAdvocate2.Username,
                            ContactEmail = _defenceAdvocate2.ContactEmail,
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        }
                    }
                });
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_defenceAdvocate1));
        }

        [Test]
        public async Task should_throw_exception_when_retry_limit_is_reached()
        {
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate3);
            var handler = async () => { await messageHandler.HandleAsync(integrationEvent); };
            await handler.Should().ThrowAsync<ArgumentException>();
        }
        
        [Test]
        public async Task should_log_error_when_conference_is_null()
        {
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync((ConferenceDetailsResponse) null);

            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate1);
            await messageHandler.HandleAsync(integrationEvent);

            _logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Unable to find conference by hearing id")),
                It.Is<Exception>(exception => exception == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Never);
        }
        
        [Test]
        public async Task Should_push_endpoints_updated_message()
        {
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate1);
            await messageHandler.HandleAsync(integrationEvent);

            _logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Unable to find defence advocate email by hearing id")),
                It.Is<Exception>(x => x == null),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(),
                It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }
        
                
        [Test]
        public async Task Should_handle_a_new_defence_advocate_rep_being_linked_and_previous_unlinked()
        {
            //Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate2);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Assert
            VideoWebServiceMock.Verify(x 
                => x.PushLinkedNewParticipantToEndpoint(_conferenceId, _defenceAdvocate2.Username, Endpoint), Times.Once);
            VideoWebServiceMock.Verify(x 
                => x.PushUnlinkedParticipantFromEndpoint(_conferenceId, _defenceAdvocate1.Username, Endpoint), Times.Once);
        }      
        
        [Test]
        public async Task Should_handle_rep_being_removed_from_endpoint()
        {
            //Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(null);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrage
            VideoWebServiceMock.Verify(x 
                => x.PushLinkedNewParticipantToEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x 
                => x.PushUnlinkedParticipantFromEndpoint(_conferenceId, _defenceAdvocate1.Username, Endpoint), Times.Once);

        }           
        
        [Test]
        public async Task Should_handle_rep_being_changed_whilst_in_a_consultation_without_new_rep()
        {
            //Arrange
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_defenceAdvocate2, EndpointState.InConsultation));
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Id = _conferenceId,
                    HearingId = _hearingId,
                    Participants = new List<ParticipantResponse>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = _defenceAdvocate1.Username,
                            ContactEmail = _defenceAdvocate1.ContactEmail,
                            CurrentStatus = ParticipantState.Available
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = _defenceAdvocate2.Username,
                            ContactEmail = _defenceAdvocate2.ContactEmail,
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        }
                    }
                });
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate1);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x
                => x.PushCloseConsultationBetweenEndpointAndParticipant(_conferenceId, _defenceAdvocate2.Username, Endpoint), Times.Once);

        }
        
        [Test]
        public async Task Should_handle_rep_being_changed_whilst_in_a_consultation_with_new_rep()
        {
            //Arrange
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_defenceAdvocate1, EndpointState.InConsultation));
            
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate2);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x
                => x.PushCloseConsultationBetweenEndpointAndParticipant(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
                
        [Test]
        public async Task Should_handle_rep_being_unlinked_whilst_in_a_consultation_with_but_no_new_rep()
        {
            //Arrange
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_defenceAdvocate1, EndpointState.InConsultation));
            
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(null);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x
                => x.PushCloseConsultationBetweenEndpointAndParticipant(_conferenceId, _defenceAdvocate1.Username, Endpoint), Times.Once);
        }
        
        [Test]
          public async Task Should_log_error_when_cant_find_defence_advocate_but_handle_rest_of_process()
        {
            //Arrange
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_defenceAdvocate2, EndpointState.InConsultation));
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Id = _conferenceId,
                    HearingId = _hearingId,
                    Participants = new List<ParticipantResponse>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = _defenceAdvocate1.Username,
                            ContactEmail = _defenceAdvocate1.ContactEmail,
                            CurrentStatus = ParticipantState.Available
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Username = "",
                            ContactEmail = "",
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        }
                    }
                });
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate1);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);

            //Arrange
            _logger.Verify(x => x.Log(
                It.Is<LogLevel>(log => log == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString().Contains("Error notifying defence advocates")),
                It.Is<ArgumentException>(ex => ex.Message.Contains($"Unable to find defence advocate in participant list {_defenceAdvocate2.Username}")),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);

        }
          
        [Test]
        public async Task Should_handle_defence_advocate_unchanged()
        {
            // Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_defenceAdvocate1);
            
            // Act
            await messageHandler.HandleAsync(integrationEvent);
            
            // Assert
            AssertNoNotificationsSent();
        }

        private void AssertNoNotificationsSent()
        {
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushUnlinkedParticipantFromEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushCloseConsultationBetweenEndpointAndParticipant(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}