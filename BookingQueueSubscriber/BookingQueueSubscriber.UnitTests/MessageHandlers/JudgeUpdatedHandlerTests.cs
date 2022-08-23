using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class JudgeUpdatedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_send_notification()
        {
            var messageHandler = new JudgeUpdatedHandler(UserCreationAndNotificationMock.Object);

            var integrationEvent = new JudgeUpdatedIntegrationEvent() { Hearing = new HearingDto(), Judge = new ParticipantDto()};
            await messageHandler.HandleAsync(integrationEvent);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Once);
        }
    }
}