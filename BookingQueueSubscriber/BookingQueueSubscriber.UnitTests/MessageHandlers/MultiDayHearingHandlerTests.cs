using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.V1.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class MultiDayHearingHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_SendHearingAmendmentNotificationAsync_when_request_is_valid()
        {
            var messageHandler = new MultiDayHearingHandler(NotificationServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            NotificationServiceMock.Verify(x => x.SendMultiDayHearingNotificationAsync(It.IsAny<HearingDto>(),
                It.IsAny<IList<ParticipantDto>>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task should_call_SendHearingAmendmentNotificationAsync_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new MultiDayHearingHandler(NotificationServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            NotificationServiceMock.Verify(x => x.SendMultiDayHearingNotificationAsync(It.Is<HearingDto>(
                request => request.ScheduledDateTime == integrationEvent.Hearing.ScheduledDateTime), 
                It.Is<IList<ParticipantDto>>
            (
                request =>
                    request.Count == integrationEvent.Participants.Count &&
                    request[0].Username == integrationEvent.Participants[0].Username &&
                    request[0].FirstName == integrationEvent.Participants[0].FirstName &&
                    request[0].LastName == integrationEvent.Participants[0].LastName &&
                    request[0].ContactEmail == integrationEvent.Participants[0].ContactEmail &&
                    request[0].ContactTelephone == integrationEvent.Participants[0].ContactTelephone &&
                    request[0].DisplayName == integrationEvent.Participants[0].DisplayName &&
                    request[0].UserRole.ToString() == integrationEvent.Participants[0].UserRole &&
                    request[0].HearingRole == integrationEvent.Participants[0].HearingRole &&
                    request[0].Representee == integrationEvent.Participants[0].Representee
            ),
                It.Is<int>(request => request == integrationEvent.TotalDays)), Times.Once);
        }

        private MultiDayHearingIntegrationEvent GetIntegrationEvent()
        {
            return new MultiDayHearingIntegrationEvent
            {
                Hearing = GetHearingDto(), 
                TotalDays = 3,
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