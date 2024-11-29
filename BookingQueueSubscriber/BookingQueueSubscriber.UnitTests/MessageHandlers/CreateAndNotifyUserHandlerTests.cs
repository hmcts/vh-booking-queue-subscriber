using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class CreateAndNotifyUserHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_user_creation_and_notification_when_request_is_valid()
        {
            var messageHandler = new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object, VideoWebServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Participants = new List<ParticipantResponse>
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.HearingConfirmationForParticipant.ContactEmail
                        }
                    }
                });

            await messageHandler.HandleAsync(integrationEvent);

            BookingsApiClientMock.Verify(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Once);
            UserServiceMock.Verify(x => x.AssignUserToGroup(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            VerifyParticipantsUpdateMessagePushedToVideoWeb();
        }

        [Test]
        public async Task should_call_CreateUserAndNotificationAsync_when_handle_is_called_with_explicit_interface()
        {
            IMessageHandler messageHandler = new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com", UserId = "123" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Participants = new List<ParticipantResponse>
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.HearingConfirmationForParticipant.ContactEmail
                        }
                    }
                });

            await messageHandler.HandleAsync(integrationEvent);

            UserServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false), Times.Once);
            NotificationApiClientMock.Verify(x => x.SendParticipantCreatedAccountEmailAsync(It.IsAny<NotificationApi.Contract.Requests.SignInDetailsEmailRequest>()));
            VerifyParticipantsUpdateMessagePushedToVideoWeb();
        }
        
         [Test]
        public async Task should_call_HandleAssignUserToGroup_when_request_has_created_user_accounts()
        {
            IMessageHandler messageHandler = new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object, VideoWebServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com", UserId = "123" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Participants = new List<ParticipantResponse>
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.HearingConfirmationForParticipant.ContactEmail
                        }
                    }
                });

            await messageHandler.HandleAsync(integrationEvent);
            UserServiceMock.Verify(x => x.AssignUserToGroup(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            VerifyParticipantsUpdateMessagePushedToVideoWeb();
        }

        [Test]
        public async Task should_assign_users_to_correct_group_when_request_is_valid()
        {
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            var user = new User { UserId = "test", Password = "test123" };
            UserServiceMock.Setup(us => us.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false))
                    .ReturnsAsync(new Func<User>(() => user));
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse
                {
                    Participants = new List<ParticipantResponse>
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.HearingConfirmationForParticipant.ContactEmail
                        }
                    }
                });

            IMessageHandler messageHandler = new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            await messageHandler.HandleAsync(integrationEvent);

            UserServiceMock.Verify(x => x.AssignUserToGroup(user.UserId, participant.UserRole));
            VerifyParticipantsUpdateMessagePushedToVideoWeb();
        } 

        private static CreateAndNotifyUserIntegrationEvent GetIntegrationEvent()
        {
            return new CreateAndNotifyUserIntegrationEvent
            {
                HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto
                {
                    CaseName = "Case 123",
                    CaseNumber = "Case Number 123",
                    ContactEmail = "participant1@test.com",
                    ContactTelephone = "12345566",
                    DisplayName = "displayname",
                    FirstName = "firstname",
                    LastName = "lastname",
                    HearingId = Guid.NewGuid(),
                    ParticipantId = Guid.NewGuid(),
                    Representee = "",
                    ScheduledDateTime = DateTime.Now.AddDays(1),
                    Username = "first.last@test.com",
                    UserRole = UserRole.Individual.ToString()
                }
            };
        }

        private void VerifyParticipantsUpdateMessagePushedToVideoWeb()
        {
            VideoWebServiceMock.Verify(x => x.PushParticipantsUpdatedMessage(It.IsAny<Guid>(), It.IsAny<UpdateConferenceParticipantsRequest>()));
        }
    }
}