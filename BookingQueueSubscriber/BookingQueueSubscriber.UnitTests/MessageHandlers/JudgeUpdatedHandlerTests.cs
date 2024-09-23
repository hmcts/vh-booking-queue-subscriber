using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class JudgeUpdatedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<JudgeUpdatedHandler>> _logger = new Mock<ILogger<JudgeUpdatedHandler>>();
        private ConferenceDetailsResponse _mockConferenceDetailsResponse;

        [SetUp]
        public new void Setup()
        {
            _mockConferenceDetailsResponse = new ConferenceDetailsResponse{Participants = new List<ParticipantResponse>
            {
                new ParticipantResponse
                {
                    ContactEmail = "Judge@email.com"
                }
            }};
            VideoApiServiceMock.Setup(e => e.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(_mockConferenceDetailsResponse);
        }
        
        [Test]
        public async Task should_send_notification_and_update_participant_details()
        {
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, UserCreationAndNotificationMock.Object, _logger.Object);

            var integrationEvent = new JudgeUpdatedIntegrationEvent()
            {
                Hearing = new HearingDto(), 
                Judge = new ParticipantDto
                {
                    ContactEmail = "new_judge@email.com", 
                    UserRole = "Judge", 
                    HearingRole = "Judge"
                },
                SendNotification = true
            };
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Once);
        }
        
        [Test]
        public async Task should_not_send_notification_and_update_participant_details()
        {
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, UserCreationAndNotificationMock.Object, _logger.Object);

            var integrationEvent = new JudgeUpdatedIntegrationEvent()
            {
                Hearing = new HearingDto(), 
                Judge = new ParticipantDto
                {
                    ContactEmail = "new_judge@email.com", 
                    UserRole = "Judge", 
                    HearingRole = "Judge"
                },
                SendNotification = false
            };
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Never);
        }

        [Test]
        public async Task should_not_send_notification_but_still_update_participant_details()
        {
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, UserCreationAndNotificationMock.Object, _logger.Object);

            var integrationEvent = new JudgeUpdatedIntegrationEvent() { Hearing = new HearingDto(), Judge = new ParticipantDto{ContactEmail = "Judge@email.com", UserRole = "Judge", HearingRole = "Judge"}};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto[]>()), Times.Never);
        }
    }
}