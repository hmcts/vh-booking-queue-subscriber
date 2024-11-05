using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using NotificationApi.Contract.Requests;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class JudgeUpdatedHandlerTests : MessageHandlerTestBase
    {
        private readonly Mock<ILogger<JudgeUpdatedHandler>> _logger = new();
        private ConferenceDetailsResponse _mockConferenceDetailsResponse;

        [SetUp]
        public new void Setup()
        {
            _mockConferenceDetailsResponse = new ConferenceDetailsResponse{Participants = new List<ParticipantResponse>
            {
                new()
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
            NotificationApiClientMock
                .Setup(c => c.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                        It.IsAny<ExistingUserSingleDayHearingConfirmationRequest>()))
                .Returns(Task.CompletedTask);
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, NotificationApiClientMock.Object, _logger.Object);
            
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()),
                Times.Once
            );
            NotificationApiClientMock.Verify(
                x => x.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                    It.Is<ExistingUserSingleDayHearingConfirmationRequest>(r =>
                        r.HearingId == integrationEvent.Hearing.HearingId &&
                        r.ContactEmail == integrationEvent.Judge.ContactEmail)
                ),
                Times.Once
            );
        }
        
        [Test]
        public async Task should_not_send_notification_and_update_participant_details()
        {
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
            NotificationApiClientMock
                .Setup(c => c.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                    It.IsAny<ExistingUserSingleDayHearingConfirmationRequest>()))
                .Returns(Task.CompletedTask);
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, NotificationApiClientMock.Object, _logger.Object);
            
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), 
                Times.Once
            );
            NotificationApiClientMock.Verify(
                x => x.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                    It.IsAny<ExistingUserSingleDayHearingConfirmationRequest>()
                ), 
                Times.Never
            );
        }

        [Test]
        public async Task should_not_send_notification_but_still_update_participant_details()
        {
            NotificationApiClientMock
                .Setup(c => c.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                    It.IsAny<ExistingUserSingleDayHearingConfirmationRequest>()))
                .Returns(Task.CompletedTask);
            var messageHandler = new JudgeUpdatedHandler(VideoApiServiceMock.Object, NotificationApiClientMock.Object, _logger.Object);
            var integrationEvent = new JudgeUpdatedIntegrationEvent() { Hearing = new HearingDto(), Judge = new ParticipantDto{ContactEmail = "Judge@email.com", UserRole = "Judge", HearingRole = "Judge"}};
            
            await messageHandler.HandleAsync(integrationEvent);
            
            VideoApiServiceMock.Verify(
                x => x.UpdateParticipantDetails(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateParticipantRequest>()), 
                Times.Once
            );
            NotificationApiClientMock.Verify(
                x => x.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(
                    It.IsAny<ExistingUserSingleDayHearingConfirmationRequest>()
                ), 
                Times.Never
            );
        }
    }
}