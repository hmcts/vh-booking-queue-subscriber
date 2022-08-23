using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class JudgeUpdatedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<JudgeUpdatedHandler>> logger = new Mock<ILogger<JudgeUpdatedHandler>>();
        [Test]
        public async Task should_send_notification_and_update_participant_details()
        {
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, UserCreationAndNotificationMock.Object, logger.Object);

            var integrationEvent = new JudgeUpdatedIntegrationEvent() { Hearing = new HearingDto(), Judge = new ParticipantDto()};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Once);
        }
    }
}