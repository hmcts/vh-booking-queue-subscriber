using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using LinkedParticipantType = BookingQueueSubscriber.Services.MessageHandlers.Dtos.LinkedParticipantType;
using BookingsApi.Contract.V1.Enums;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantsAddedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantsAddedHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, new Mock<ILogger<ParticipantsAddedHandler>>().Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.IsAny<AddParticipantsToConferenceRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushParticipantsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceParticipantsRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new ParticipantsAddedHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, new Mock<ILogger<ParticipantsAddedHandler>>().Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.Is<AddParticipantsToConferenceRequest>
            (
                request => 
                    request.Participants.Count == 1 &&
                    request.Participants[0].Name == integrationEvent.Participants[0].Fullname &&
                    request.Participants[0].Username == integrationEvent.Participants[0].Username &&
                    request.Participants[0].FirstName == integrationEvent.Participants[0].FirstName &&
                    request.Participants[0].LastName == integrationEvent.Participants[0].LastName &&
                    request.Participants[0].ContactEmail == integrationEvent.Participants[0].ContactEmail &&
                    request.Participants[0].ContactTelephone == integrationEvent.Participants[0].ContactTelephone &&
                    request.Participants[0].DisplayName == integrationEvent.Participants[0].DisplayName &&
                    request.Participants[0].UserRole.ToString() == integrationEvent.Participants[0].UserRole &&
                    request.Participants[0].HearingRole == integrationEvent.Participants[0].HearingRole &&
                    request.Participants[0].CaseTypeGroup == integrationEvent.Participants[0].CaseGroupType.ToString() &&
                    request.Participants[0].ParticipantRefId == integrationEvent.Participants[0].ParticipantId &&
                    request.Participants[0].Representee == integrationEvent.Participants[0].Representee
            )), Times.Once);
        }
        
         [Test]
        public async Task should_call_video_api_when_request_has_linked_participants_and_is_valid()
        {
            var messageHandler = new ParticipantsAddedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object,
                new Mock<ILogger<ParticipantsAddedHandler>>().Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.IsAny<AddParticipantsToConferenceRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_request_has_linked_participants_and_handler_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new ParticipantsAddedHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, new Mock<ILogger<ParticipantsAddedHandler>>().Object);

            var integrationEvent = GetIntegrationEventWithLinkedParticipant();
            var dtoList = MapToRequestFromDto(integrationEvent.Participants[0].LinkedParticipants);
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.Is<AddParticipantsToConferenceRequest>
            (
                request => 
                    request.Participants.Count == 1 &&
                    request.Participants[0].Name == integrationEvent.Participants[0].Fullname &&
                    request.Participants[0].Username == integrationEvent.Participants[0].Username &&
                    request.Participants[0].FirstName == integrationEvent.Participants[0].FirstName &&
                    request.Participants[0].LastName == integrationEvent.Participants[0].LastName &&
                    request.Participants[0].ContactEmail == integrationEvent.Participants[0].ContactEmail &&
                    request.Participants[0].ContactTelephone == integrationEvent.Participants[0].ContactTelephone &&
                    request.Participants[0].DisplayName == integrationEvent.Participants[0].DisplayName &&
                    request.Participants[0].UserRole.ToString() == integrationEvent.Participants[0].UserRole &&
                    request.Participants[0].HearingRole == integrationEvent.Participants[0].HearingRole &&
                    request.Participants[0].CaseTypeGroup == integrationEvent.Participants[0].CaseGroupType.ToString() &&
                    request.Participants[0].ParticipantRefId == integrationEvent.Participants[0].ParticipantId &&
                    request.Participants[0].Representee == integrationEvent.Participants[0].Representee &&
                    request.Participants[0].LinkedParticipants.Count.Equals(1) &&
                    request.Participants[0].LinkedParticipants[0].Type == dtoList[0].Type &&
                    request.Participants[0].LinkedParticipants[0].ParticipantRefId == dtoList[0].ParticipantRefId &&
                    request.Participants[0].LinkedParticipants[0].LinkedRefId == dtoList[0].LinkedRefId
            )), Times.Once);
        }

        private ParticipantsAddedIntegrationEvent GetIntegrationEvent()
        {

            return new ParticipantsAddedIntegrationEvent
            {
                Hearing = GetHearingDto(),
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        CaseGroupType = CaseRoleGroup.Applicant,
                        DisplayName = "displayName",
                        Fullname = "fullname",
                        FirstName = "firstName",
                        LastName = "lastName",
                        ContactEmail = "test@hmcts.net",
                        ContactTelephone = "012748465859",
                        HearingRole = "hearingRole",
                        ParticipantId = ParticipantId,
                        Representee = "representee",
                        UserRole = UserRole.Individual.ToString(),
                        Username = "username",
                        LinkedParticipants = new List<LinkedParticipantDto>()
                    }
                }
            };
        }
        
        private ParticipantsAddedIntegrationEvent GetIntegrationEventWithLinkedParticipant()
        {
            return new ParticipantsAddedIntegrationEvent
            {
                Hearing = GetHearingDto(),
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        ParticipantId = ParticipantId,
                        CaseGroupType = CaseRoleGroup.Applicant,
                        DisplayName = "displayName",
                        Fullname = "fullname",
                        FirstName = "firstName",
                        LastName = "lastName",
                        ContactEmail = "test@email.com",
                        ContactTelephone = "012748465859",
                        HearingRole = "hearingRole",
                        Representee = "representee",
                        UserRole = UserRole.Individual.ToString(),
                        Username = "username",
                        LinkedParticipants = new List<LinkedParticipantDto>{ 
                            new LinkedParticipantDto
                            {
                                LinkedId = Guid.NewGuid(),
                                ParticipantId = Guid.NewGuid(),
                                Type = LinkedParticipantType.Interpreter
                            }}
                    }
                }
            };
        }

        private static HearingDto GetHearingDto()
        {
            return new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Generic",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true
            };
        }

        private IList<LinkedParticipantRequest> MapToRequestFromDto(IList<LinkedParticipantDto> linked)
        {
            return linked.Select(l => new LinkedParticipantRequest()
            {
                LinkedRefId = l.LinkedId,
                ParticipantRefId = l.ParticipantId,
                Type = Enum.Parse<VideoApi.Contract.Enums.LinkedParticipantType>(l.Type.ToString())
            }).ToList();
        }
    }
}