using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantsAddedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantsAddedHandler(VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.IsAny<AddParticipantsToConferenceRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new ParticipantsAddedHandler(VideoApiServiceMock.Object);

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

        private ParticipantsAddedIntegrationEvent GetIntegrationEvent()
        {
            return new ParticipantsAddedIntegrationEvent
            {
                HearingId = HearingId,
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        CaseGroupType = CaseRoleGroup.Applicant,
                        DisplayName = "displayName",
                        Fullname = "fullname",
                        FirstName = "firstName",
                        LastName = "lastName",
                        ContactEmail = "test@email.com",
                        ContactTelephone = "012748465859",
                        HearingRole = "hearingRole",
                        ParticipantId = ParticipantId,
                        Representee = "representee",
                        UserRole = UserRole.Individual.ToString(),
                        Username = "username"
                    }
                }
            };
        }
    }
}