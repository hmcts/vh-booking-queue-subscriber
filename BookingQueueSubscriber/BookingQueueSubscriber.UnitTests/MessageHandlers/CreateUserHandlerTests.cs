using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class CreateUserHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_create_user_when_request_is_valid()
        {
            // Arrange
            var messageHandler = new CreateUserHandler(UserServiceMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.Participant;
            var hearingId = participant.HearingId;
            const string username = "test.com";
            var contactEmail = participant.ContactEmail;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = username });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.Participant.ContactEmail
                        }
                    }
                });
            
            // Act
            await messageHandler.HandleAsync(integrationEvent);
            
            // Assert
            BookingsApiClientMock.Verify(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Once);
            UserServiceMock.Verify(x => x.AssignUserToGroup(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            VideoApiServiceMock.Verify(x => x.UpdateParticipantUsernameWithPolling(hearingId, username, contactEmail));
        }
        
        private CreateUserIntegrationEvent GetIntegrationEvent()
        {

            return new CreateUserIntegrationEvent
            {
                Participant = new ParticipantUserDto
                {
                    ContactEmail = "participant1@test.com",
                    FirstName = "firstname",
                    LastName = "lastname",
                    HearingId = Guid.NewGuid(),
                    Username = "first.last@test.com",
                    UserRole = UserRole.Individual.ToString()
                }
            };
        }
    }
}
