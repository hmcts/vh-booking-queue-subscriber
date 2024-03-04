using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class JudgeUpdatedNoNotificationHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<JudgeUpdatedNoNotificationHandler>> _logger = new Mock<ILogger<JudgeUpdatedNoNotificationHandler>>();
        private ConferenceDetailsResponse _mockConferenceDetailsResponse;

        [SetUp]
        public new void Setup()
        {
            _mockConferenceDetailsResponse = new ConferenceDetailsResponse{Participants = new List<ParticipantDetailsResponse>
            {
                new ParticipantDetailsResponse
                {
                    ContactEmail = "Judge@email.com"
                }
            }};
            VideoApiServiceMock.Setup(e => e.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(_mockConferenceDetailsResponse);
        }
        
        [Test]
        public async Task should_update_participant_details()
        {
            var messageHandler = new JudgeUpdatedNoNotificationHandler(VideoApiServiceMock.Object, _logger.Object);

            var integrationEvent = new JudgeUpdatedNoNotificationIntegrationEvent() { Hearing = new HearingDto(), Judge = new ParticipantDto{ContactEmail = "new_judge@email.com", UserRole = "Judge", HearingRole = "Judge"}};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Never);
        }
        
    }
}