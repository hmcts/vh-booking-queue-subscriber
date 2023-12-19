using System.Net;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.V1.Enums;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class CreateAndNotifyUserHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_usercreation_and_notification_when_request_is_valid()
        {
            var messageHandler = new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
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
        }

        [Test]
        public async Task should_call_CreateUserAndNotifcationAsync_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com", UserId = "123" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
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
        }
        
         [Test]
        public async Task should_call_HandleAssignUserToGroup_when_request_has_created_useraccounts()
        {
            var messageHandler = (IMessageHandler)new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object);
            var integrationEvent = GetIntegrationEvent();
            var participant = integrationEvent.HearingConfirmationForParticipant;
            UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, false)).ReturnsAsync(
                new User { UserName = "test.com", UserId = "123" });
            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
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
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
                    {
                        new()
                        {
                            Id = ParticipantId,
                            ContactEmail = integrationEvent.HearingConfirmationForParticipant.ContactEmail
                        }
                    }
                });

            var messageHandler = (IMessageHandler)new CreateAndNotifyUserHandler(UserServiceMock.Object, NotificationApiClientMock.Object, BookingsApiClientMock.Object, VideoApiServiceMock.Object);

            await messageHandler.HandleAsync(integrationEvent);

            UserServiceMock.Verify(x => x.AssignUserToGroup(user.UserId, participant.UserRole));
        } 
        
    [Test]
    public async Task should_poll_video_api_for_response_then_throw_error()
    {
        var messageHandler = new CreateAndNotifyUserHandler(
            UserServiceMock.Object,
            NotificationApiClientMock.Object, 
            BookingsApiClientMock.Object, 
            VideoApiServiceMock.Object);

        UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new User() { UserName = "username"});
        
        //video mock should throw not found exception
        VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null));
        
        var integrationEvent = new CreateAndNotifyUserIntegrationEvent
        {
            HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto
            {
                HearingId = HearingId,
                ParticipantId = ParticipantId,
                ContactEmail = "email@email.com",
                FirstName = "John",
                LastName = "Smith",
                UserRole = "Individual",
                CaseName = "Case Name",
                CaseNumber = "1234567890",
                ScheduledDateTime = DateTime.UtcNow
            }
        };
        //assert that message handler throws exception
        Assert.ThrowsAsync<VideoApiException>(() => messageHandler.HandleAsync(integrationEvent));
    }
    
    [Test]
    public async Task should_poll_video_api_for_response_then_return_it_after_initial_error()
    {
        var messageHandler = new CreateAndNotifyUserHandler(
            UserServiceMock.Object,
            NotificationApiClientMock.Object, 
            BookingsApiClientMock.Object, 
            VideoApiServiceMock.Object);

        UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new User() { UserName = "username"});
        
        //video mock should throw not found exception, then return conference on second iteration
        VideoApiServiceMock.SetupSequence(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null))
            .ReturnsAsync(new ConferenceDetailsResponse()
            {
                Participants = new List<ParticipantDetailsResponse>()
                {
                    new()
                    {
                        Id = ParticipantId,
                        ContactEmail = "email@email.com"
                    }
                }
            });
            
        
        var integrationEvent = new CreateAndNotifyUserIntegrationEvent
        {
            HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto
            {
                HearingId = HearingId,
                ParticipantId = ParticipantId,
                ContactEmail = "email@email.com",
                FirstName = "John",
                LastName = "Smith",
                UserRole = "Individual",
                CaseName = "Case Name",
                CaseNumber = "1234567890",
                ScheduledDateTime = DateTime.UtcNow
            }
        };
        await messageHandler.HandleAsync(integrationEvent);
        //assert message handler does not throw exception
        Assert.Pass();
    }

        private CreateAndNotifyUserIntegrationEvent GetIntegrationEvent()
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
    }
}