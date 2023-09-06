using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using BookingsApi.Contract.V1.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantUpdatedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<ParticipantUpdatedHandler>> _logger =
            new Mock<ILogger<ParticipantUpdatedHandler>>();

        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<UpdateParticipantRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler =
                (IMessageHandler) new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.Is<UpdateParticipantRequest>
                (
                    request =>
                        request.DisplayName == integrationEvent.Participant.DisplayName &&
                        request.FirstName == integrationEvent.Participant.FirstName &&
                        request.LastName == integrationEvent.Participant.LastName &&
                        request.Representee == integrationEvent.Participant.Representee &&
                        request.ContactEmail == integrationEvent.Participant.ContactEmail &&
                        request.ContactTelephone == integrationEvent.Participant.ContactTelephone &&
                        request.Fullname == integrationEvent.Participant.Fullname &&
                        request.Username == integrationEvent.Participant.Username
                )), Times.Once);
        }
        
        [Test]
        public async Task should_call_video_api_when_request_is_valid_with_linked_participant()
        {
            var messageHandler = new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _logger.Object);

            var integrationEvent = GetIntegrationEventWithLinkedParticipant();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<UpdateParticipantRequest>()), Times.Once);
        }
        
        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface_with_linked_participant()
        {
            var messageHandler =
                (IMessageHandler) new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _logger.Object);
            var integrationEvent = GetIntegrationEventWithLinkedParticipant();
            var dtoList = MapToRequestFromDto(integrationEvent.Participant.LinkedParticipants);

            await messageHandler.HandleAsync(integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.Is<UpdateParticipantRequest>
                (
                    request =>
                        request.DisplayName == integrationEvent.Participant.DisplayName &&
                        request.FirstName == integrationEvent.Participant.FirstName &&
                        request.LastName == integrationEvent.Participant.LastName &&
                        request.Representee == integrationEvent.Participant.Representee &&
                        request.ContactEmail == integrationEvent.Participant.ContactEmail &&
                        request.ContactTelephone == integrationEvent.Participant.ContactTelephone &&
                        request.Fullname == integrationEvent.Participant.Fullname &&
                        request.Username == integrationEvent.Participant.Username &&
                        request.LinkedParticipants.Count.Equals(1) &&
                        request.LinkedParticipants[0].Type == dtoList[0].Type &&
                        request.LinkedParticipants[0].ParticipantRefId == dtoList[0].ParticipantRefId &&
                        request.LinkedParticipants[0].LinkedRefId == dtoList[0].LinkedRefId
                )), Times.Once);
        }

        private ParticipantUpdatedIntegrationEvent GetIntegrationEvent()
        {
            return new ParticipantUpdatedIntegrationEvent
            {
                HearingId = HearingId, Participant = new ParticipantDto
                {
                    ParticipantId = ParticipantId,
                    CaseGroupType = CaseRoleGroup.Applicant,
                    DisplayName = "displayName",
                    Fullname = "fullname",
                    FirstName = "firstName",
                    LastName = "lastName",
                    ContactEmail = "test@hmcts.net",
                    ContactTelephone = "012748465859",
                    HearingRole = "hearingRole",
                    Representee = "representee",
                    UserRole = "Individual",
                    Username = "username"
                }
            };
        }

        private ParticipantUpdatedIntegrationEvent GetIntegrationEventWithLinkedParticipant()
        {
            var integrationEvent = GetIntegrationEvent();
            integrationEvent.Participant.LinkedParticipants = new List<LinkedParticipantDto>
            {
                new LinkedParticipantDto
                {
                    ParticipantId = ParticipantId,
                    LinkedId = Guid.NewGuid(),
                    Type = Services.MessageHandlers.Dtos.LinkedParticipantType.Interpreter
                }
            };
            return integrationEvent;
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