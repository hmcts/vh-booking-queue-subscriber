using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Moq;
using NUnit.Framework;

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
            VideoApiServiceMock.Verify(x => x.AddParticipantsToConference(It.IsAny<Guid>(), It.IsAny<AddParticipantsToConferenceRequest>()), Times.Once);
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
                        DisplayName = "name",
                        Fullname = "fullname",
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