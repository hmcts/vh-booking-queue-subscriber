using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingParticipantsUpdatedHandlerTests : MessageHandlerTestBase
    {
        private HearingParticipantsUpdatedIntegrationEvent _integrationEvent;

        private HearingParticipantsUpdatedHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _integrationEvent = GetIntegrationEvent();
            _handler = new HearingParticipantsUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);
        }


        [Test]
        public async Task should_call_video_api_to_retrieve_conference()
        {
            await _handler.HandleAsync(_integrationEvent);

            VideoApiServiceMock.Verify(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_to_update_conference_participants()
        {
            await _handler.HandleAsync(_integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateConferenceParticipantsAsync(ConferenceDetailsResponse.Id, 
                It.Is<UpdateConferenceParticipantsRequest>(r =>
                    r.ExistingParticipants[0].ContactEmail == _integrationEvent.ExistingParticipants[0].ContactEmail
                    && r.ExistingParticipants[0].DisplayName == _integrationEvent.ExistingParticipants[0].DisplayName
                    && r.ExistingParticipants[0].ParticipantRefId == _integrationEvent.ExistingParticipants[0].ParticipantId
                    && r.ExistingParticipants[0].Username == _integrationEvent.ExistingParticipants[0].Username

                    && r.NewParticipants[0].ContactEmail == _integrationEvent.NewParticipants[0].ContactEmail
                    && r.NewParticipants[0].DisplayName == _integrationEvent.NewParticipants[0].DisplayName
                    && r.NewParticipants[0].HearingRole == _integrationEvent.NewParticipants[0].HearingRole
                    && r.NewParticipants[0].ParticipantRefId == _integrationEvent.NewParticipants[0].ParticipantId
                    && r.NewParticipants[0].UserRole == VideoApi.Contract.Enums.UserRole.Individual
                    && r.NewParticipants[0].Username == _integrationEvent.NewParticipants[0].Username

                    && r.RemovedParticipants == _integrationEvent.RemovedParticipants

                    && r.LinkedParticipants[0].LinkedRefId == _integrationEvent.LinkedParticipants[0].LinkedId
                    && r.LinkedParticipants[0].ParticipantRefId == _integrationEvent.LinkedParticipants[0].ParticipantId
                    && r.LinkedParticipants[0].Type == VideoApi.Contract.Enums.LinkedParticipantType.Interpreter
            )), Times.Once);
        }

        private HearingParticipantsUpdatedIntegrationEvent GetIntegrationEvent()
        {
            var hearingDto = new HearingDto
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
            return new HearingParticipantsUpdatedIntegrationEvent
            {
                Hearing = hearingDto,
                ExistingParticipants = new List<ParticipantDto>
                {
                    new()
                    {
                        ParticipantId = ParticipantId,
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
                },
                NewParticipants = new List<ParticipantDto>
                {
                    new()
                    {
                        ParticipantId = ParticipantId,
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
                },
                RemovedParticipants = new List<Guid> { Guid.NewGuid() },
                LinkedParticipants = new List<LinkedParticipantDto>
                {
                    new()
                    {
                        LinkedId = Guid.NewGuid(),
                        ParticipantId = Guid.NewGuid(),
                        Type = LinkedParticipantType.Interpreter
                    }
                },
            };
        }
    }
}