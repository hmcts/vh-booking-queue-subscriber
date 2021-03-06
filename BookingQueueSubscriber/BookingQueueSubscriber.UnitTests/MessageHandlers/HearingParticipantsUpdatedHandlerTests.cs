using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            VideoApiServiceMock.Verify(x => x.GetConferenceByHearingRefId(HearingId, true), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_to_update_conference_participants()
        {
            await _handler.HandleAsync(_integrationEvent);

            VideoApiServiceMock.Verify(x => x.UpdateConferenceParticipantsAsync(ConferenceDetailsResponse.Id, 
                It.Is<UpdateConferenceParticipantsRequest>(r =>
                    r.ExistingParticipants[0].ContactEmail == _integrationEvent.ExistingParticipants[0].ContactEmail
                    && r.ExistingParticipants[0].ContactTelephone == _integrationEvent.ExistingParticipants[0].ContactTelephone
                    && r.ExistingParticipants[0].DisplayName == _integrationEvent.ExistingParticipants[0].DisplayName
                    && r.ExistingParticipants[0].FirstName == _integrationEvent.ExistingParticipants[0].FirstName
                    && r.ExistingParticipants[0].Fullname == _integrationEvent.ExistingParticipants[0].Fullname
                    && r.ExistingParticipants[0].LastName == _integrationEvent.ExistingParticipants[0].LastName
                    && r.ExistingParticipants[0].ParticipantRefId == _integrationEvent.ExistingParticipants[0].ParticipantId
                    && r.ExistingParticipants[0].Representee == _integrationEvent.ExistingParticipants[0].Representee
                    && r.ExistingParticipants[0].Username == _integrationEvent.ExistingParticipants[0].Username

                    && r.NewParticipants[0].CaseTypeGroup == _integrationEvent.NewParticipants[0].CaseGroupType.ToString()
                    && r.NewParticipants[0].ContactEmail == _integrationEvent.NewParticipants[0].ContactEmail
                    && r.NewParticipants[0].ContactTelephone == _integrationEvent.NewParticipants[0].ContactTelephone
                    && r.NewParticipants[0].DisplayName == _integrationEvent.NewParticipants[0].DisplayName
                    && r.NewParticipants[0].FirstName == _integrationEvent.NewParticipants[0].FirstName
                    && r.NewParticipants[0].HearingRole == _integrationEvent.NewParticipants[0].HearingRole
                    && r.NewParticipants[0].LastName == _integrationEvent.NewParticipants[0].LastName
                    && r.NewParticipants[0].Name == _integrationEvent.NewParticipants[0].Fullname
                    && r.NewParticipants[0].ParticipantRefId == _integrationEvent.NewParticipants[0].ParticipantId
                    && r.NewParticipants[0].Representee == _integrationEvent.NewParticipants[0].Representee
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
            return new HearingParticipantsUpdatedIntegrationEvent
            {
                HearingId = HearingId,
                ExistingParticipants = new List<ParticipantDto>
                {
                    new ParticipantDto
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
                        UserRole = "useRole",
                        Username = "username"
                    }
                },
                NewParticipants = new List<ParticipantDto>
                {
                    new ParticipantDto
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
                },
                RemovedParticipants = new List<Guid> { Guid.NewGuid() },
                LinkedParticipants = new List<LinkedParticipantDto>
                {
                    new LinkedParticipantDto
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