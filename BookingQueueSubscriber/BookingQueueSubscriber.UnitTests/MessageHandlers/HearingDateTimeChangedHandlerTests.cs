using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.V1.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingDateTimeChangedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_services_when_request_is_valid()
        {
            var messageHandler = new HearingDateTimeChangedHandler(NotificationServiceMock.Object, VideoWebServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            NotificationServiceMock.Verify(x => x.SendHearingAmendmentNotificationAsync(It.IsAny<HearingDto>(),It.IsAny<DateTime>(),
                It.IsAny<IList<ParticipantDto>>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingDateTimeChangedMessage(integrationEvent.Hearing.HearingId));
        }

        [Test]
        public async Task should_call_services_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new HearingDateTimeChangedHandler(NotificationServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);

            NotificationServiceMock.Verify(x => x.SendHearingAmendmentNotificationAsync(It.Is<HearingDto>(
                request => request.ScheduledDateTime == integrationEvent.Hearing.ScheduledDateTime), 
                It.Is<DateTime>(request => request == integrationEvent.OldScheduledDateTime), 
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
            )), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingDateTimeChangedMessage(integrationEvent.Hearing.HearingId));
        }

        private HearingDateTimeChangedIntegrationEvent GetIntegrationEvent()
        {

            return new HearingDateTimeChangedIntegrationEvent
            {
                Hearing = GetHearingDto(), 
                OldScheduledDateTime = DateTime.UtcNow.AddDays(-1),
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
    }
}