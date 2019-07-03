using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantUpdatedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantUpdatedHandler(VideoApiServiceMock.Object);

            var integrationEvent = new ParticipantUpdatedIntegrationEvent { HearingId = HearingId, Participant = new ParticipantDto
            {
                CaseGroupType = CaseRoleGroup.Applicant,
                DisplayName = "name",
                Fullname = "fullname",
                HearingRole = "hearingRole",
                ParticipantId = ParticipantId,
                Representee = "representee",
                UserRole = "userole",
                Username = "username"
            }};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
        }
    }
}