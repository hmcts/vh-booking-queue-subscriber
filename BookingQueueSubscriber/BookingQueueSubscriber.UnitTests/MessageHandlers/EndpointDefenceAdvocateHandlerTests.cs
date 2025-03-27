using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Responses;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
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

        private readonly ParticipantResponse _endpointRepresentative1 = new() { Id = Guid.NewGuid(), Username = "rep1Username", ContactEmail = "rep1ContactEmail" };
        private readonly ParticipantResponse _endpointRepresentative2 = new() { Id = Guid.NewGuid(), Username = "rep2Username@email.com", ContactEmail = "rep2ContactEmail@email.com" };
        private readonly ParticipantResponse _endpointRepresentative3 = new() { Id = Guid.NewGuid(), Username = "rep3Username@email.com", ContactEmail = "rep3ContactEmail@email.com" };

        private EndpointUpdatedIntegrationEvent GetIntegrationEventValid() => GetIntegrationEventValid(new List<ParticipantResponse>());

        private EndpointUpdatedIntegrationEvent GetIntegrationEventValid(ParticipantResponse endpointRepresentative) => GetIntegrationEventValid(new List<ParticipantResponse> { endpointRepresentative });
        
        private EndpointUpdatedIntegrationEvent GetIntegrationEventValid(List<ParticipantResponse> endpointRepresentatives)
        {
            return new EndpointUpdatedIntegrationEvent
            {
                HearingId = _hearingId,
                Sip = SipAddress,
                DisplayName = Endpoint,
                ParticipantsLinked = endpointRepresentatives.Select(x => x.ContactEmail).ToList(),
            };
        }
        
        private List<EndpointResponse> GetEndpointsForConference(string endpointRepresentative, EndpointState state = EndpointState.Connected) => GetEndpointsForConference(new List<string>{endpointRepresentative}, state);
        
        private List<EndpointResponse> GetEndpointsForConference(List<string> endpointRepresentatives, EndpointState state = EndpointState.Connected)
        {
            return new List<EndpointResponse>
            {
                new()
                {
                    Id = _hearingId,
                    SipAddress = SipAddress,
                    DisplayName = Endpoint,
                    ParticipantsLinked = endpointRepresentatives,
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
                            Id = _endpointRepresentative1.Id,
                            Username = _endpointRepresentative1.Username,
                            ContactEmail = _endpointRepresentative1.ContactEmail,
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        },
                        new()
                        {
                            Id = _endpointRepresentative2.Id,
                            Username = _endpointRepresentative2.Username,
                            ContactEmail = _endpointRepresentative2.ContactEmail,
                            CurrentRoom = new RoomResponse {Id = 1, Label = "Private Consultation Room", Locked = false},
                            CurrentStatus = ParticipantState.InConsultation
                        }
                    }
                });
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_endpointRepresentative1.Username));
        }
        
        [Test]
        public async Task should_log_error_when_conference_is_null()
        {
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync((ConferenceDetailsResponse) null);

            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative1);
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

            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative1);
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateEndpointInConference(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UpdateEndpointRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushEndpointsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceEndpointsRequest>()), Times.Once);
        }
        
                
        [Test]
        public async Task Should_handle_a_new_defence_advocate_rep_being_linked_and_previous_unlinked()
        {
            //Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative2);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Assert
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(_conferenceId, _endpointRepresentative2.Username, Endpoint), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushUnlinkedParticipantFromEndpoint(_conferenceId, _endpointRepresentative1.Username, Endpoint), Times.Once);
        }      
        
        [Test]
        public async Task Should_handle_rep_being_removed_from_endpoint()
        {
            //Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid();
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushUnlinkedParticipantFromEndpoint(_conferenceId, _endpointRepresentative1.Username, Endpoint), Times.Once);
        }           
        
        [Test]
        public async Task Should_handle_rep_being_changed_whilst_in_a_consultation_without_new_rep()
        {
            //Arrange
            var rep2 = new ParticipantResponse()
            {
                Id = _endpointRepresentative2.Id,
                Username = _endpointRepresentative2.Username,
                ContactEmail = _endpointRepresentative2.ContactEmail,
                CurrentRoom = new RoomResponse { Id = 1, Label = "Private Consultation Room", Locked = false },
                CurrentStatus = ParticipantState.InConsultation
            };
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(rep2.Username, EndpointState.InConsultation));
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
                            Id = _endpointRepresentative1.Id,
                            Username = _endpointRepresentative1.Username,
                            ContactEmail = _endpointRepresentative1.ContactEmail,
                            CurrentStatus = ParticipantState.Available
                        },
                        rep2
                    }
                });
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative1);
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x
                => x.PushCloseConsultationBetweenEndpointAndParticipant(_conferenceId, _endpointRepresentative2.Username, Endpoint), Times.Once);

        }
        
        [Test]
        public async Task Should_handle_rep_being_changed_whilst_in_a_consultation_with_new_rep()
        {
            //Arrange
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_endpointRepresentative1.Username, EndpointState.InConsultation));
            
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative2);
            
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
            _endpointRepresentative1.CurrentStatus = ParticipantState.InConsultation;
            _endpointRepresentative1.CurrentRoom = new RoomResponse { Id = 1, Label = "Private Consultation Room", Locked = false };
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(_endpointRepresentative1.Username, EndpointState.InConsultation));
            
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid();
            
            //Act
            await messageHandler.HandleAsync(integrationEvent);
            
            //Arrange
            VideoWebServiceMock.Verify(x
                => x.PushCloseConsultationBetweenEndpointAndParticipant(_conferenceId, _endpointRepresentative1.Username, Endpoint), Times.Once);
        }
          
        [Test]
        public async Task Should_handle_defence_advocate_unchanged()
        {
            // Arrange
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(_endpointRepresentative1);
            
            // Act
            await messageHandler.HandleAsync(integrationEvent);
            
            // Assert
            AssertNoNotificationsSent();
        }
        
        [Test(Description = "2 participants linked to endpoint, 1 new participant linked, 1 existing participant unlinked, 1 participant in consultation with endpoint unlinked" +
                            "but consultation stays open because other participant already linked is in there")]
        public async Task Should_handle_multiple_reps_being_linked_and_unlinked()
        {
            // Arrange
            _endpointRepresentative1.CurrentStatus = ParticipantState.InConsultation;
            _endpointRepresentative1.CurrentRoom = new RoomResponse { Id = 1, Label = "Private Consultation Room", Locked = false };
            _endpointRepresentative2.CurrentStatus = ParticipantState.InConsultation;
            _endpointRepresentative2.CurrentRoom = new RoomResponse { Id = 1, Label = "Private Consultation Room", Locked = false };
            VideoApiServiceMock
                .Setup(e => e.GetEndpointsForConference(It.IsAny<Guid>()))
                .ReturnsAsync(GetEndpointsForConference(new List<string> { _endpointRepresentative1.Username, _endpointRepresentative2.Username }, EndpointState.InConsultation));
            VideoApiServiceMock
                .Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Id = _conferenceId,
                    HearingId = _hearingId,
                    Participants = new List<ParticipantResponse>()
                    {
                        _endpointRepresentative1,
                        _endpointRepresentative2,
                        _endpointRepresentative3
                    }
                }); 
            var messageHandler = new EndpointUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventValid(new List<ParticipantResponse> { _endpointRepresentative2, _endpointRepresentative3 });
            
            // Act
            await messageHandler.HandleAsync(integrationEvent);
            
            // Assert
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(_conferenceId, _endpointRepresentative3.Username, Endpoint), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(_conferenceId, _endpointRepresentative2.Username, Endpoint), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushUnlinkedParticipantFromEndpoint(_conferenceId, _endpointRepresentative1.Username, Endpoint), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushCloseConsultationBetweenEndpointAndParticipant(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private void AssertNoNotificationsSent()
        {
            VideoWebServiceMock.Verify(x => x.PushLinkedNewParticipantToEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushUnlinkedParticipantFromEndpoint(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            VideoWebServiceMock.Verify(x => x.PushCloseConsultationBetweenEndpointAndParticipant(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}