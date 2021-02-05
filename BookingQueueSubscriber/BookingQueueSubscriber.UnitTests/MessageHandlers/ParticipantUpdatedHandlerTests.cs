using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class ParticipantUpdatedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<EndpointRemovedHandler>> _loggerMock = new Mock<ILogger<EndpointRemovedHandler>>();

        [Test]
        public async Task should_call_video_api_when_request_is_valid()
        {
            var messageHandler = new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _loggerMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<UpdateParticipantRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new ParticipantUpdatedHandler(VideoApiServiceMock.Object, _loggerMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.Is<UpdateParticipantRequest>
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

        private ParticipantUpdatedIntegrationEvent GetIntegrationEvent()
        {
            return new ParticipantUpdatedIntegrationEvent { HearingId = HearingId, Participant = new ParticipantDto
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
                UserRole = "useRole",
                Username = "username"
            }};
        }
    }
}