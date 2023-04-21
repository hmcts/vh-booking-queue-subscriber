using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.Enums;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class CreateAndNotifyUserHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_usercreation_and_notification_when_request_is_valid()
        {
            var messageHandler = new CreateAndNotifyUserHandler(UserCreationAndNotificationMock.Object);
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            UserCreationAndNotificationMock.Verify(x => x.CreateUserAndNotifcationAsync(It.IsAny<HearingDto>(), It.IsAny<IList<ParticipantDto>>()), Times.Once);
            UserCreationAndNotificationMock.Verify(x => x.HandleAssignUserToGroup(It.IsAny<IList<UserDto>>()), Times.Once);
        }

        [Test]
        public async Task should_call_CreateUserAndNotifcationAsync_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new CreateAndNotifyUserHandler(UserCreationAndNotificationMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            
            UserCreationAndNotificationMock.Verify(x => x.CreateUserAndNotifcationAsync(It.IsAny<HearingDto>(), It.Is<IList<ParticipantDto>>
            (
                request =>
                    request.Count == integrationEvent.Participants.Count &&
                    request[0].Username == integrationEvent.Participants[0].Username &&
                    request[0].FirstName == integrationEvent.Participants[0].FirstName &&
                    request[0].LastName == integrationEvent.Participants[0].LastName &&
                    request[0].ContactEmail == integrationEvent.Participants[0].ContactEmail &&
                    request[0].ContactTelephone == integrationEvent.Participants[0].ContactTelephone &&
                    request[0].DisplayName == integrationEvent.Participants[0].DisplayName &&
                    request[0].UserRole.ToString() == integrationEvent.Participants[0].UserRole &&
                    request[0].HearingRole == integrationEvent.Participants[0].HearingRole &&
                    request[0].Representee == integrationEvent.Participants[0].Representee
            )), Times.Once);
        }
        
         [Test]
        public async Task should_call_HandleAssignUserToGroup_when_request_has_created_useraccounts()
        {
            var messageHandler = (IMessageHandler)new CreateAndNotifyUserHandler(UserCreationAndNotificationMock.Object);

            var integrationEvent = GetIntegrationEvent();
            var users = new List<UserDto> { new UserDto() { UserId = "mm@mm.com", Username = "mm@mm.com", UserRole = "Judge" } };
            UserCreationAndNotificationMock.Setup(x => x.CreateUserAndNotifcationAsync(It.IsAny<HearingDto>(), It.IsAny<IList<ParticipantDto>>()))
                .ReturnsAsync(users);

            await messageHandler.HandleAsync(integrationEvent);
            UserCreationAndNotificationMock.Verify(x => x.HandleAssignUserToGroup(It.Is<IList<UserDto>>(
                request => 
                    request.Count == users.Count &&
                    request[0].UserId == users[0].UserId && 
                    request[0].Username == users[0].Username && 
                    request[0].UserRole == users[0].UserRole)), Times.Once);
        }

        [Test]
        [Ignore("TODO fix")]
        public async Task should_assign_users_to_correct_group_when_request_is_valid()
        {
            var notificationService = new Mock<INotificationService>();
            var userApiClient = new Mock<IUserApiClient>();
            var integrationEvent = new CreateAndNotifyUserIntegrationEvent
            {
                Hearing = GetHearingDto(),
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.Representative.ToString(),
                        UserRole = "Representative"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.JudicialOfficeHolder.ToString(),
                        UserRole = "Judicial Office Holder"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.StaffMember.ToString(),
                        UserRole = "StaffMember"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.Individual.ToString(),
                        UserRole = "Individual"
                    }
                }
            };

            foreach (var participant in integrationEvent.Participants)
            {
                userApiClient.Setup(x => x.CreateUserAsync(It.Is<CreateUserRequest>(r => r.LastName == participant.LastName))).ReturnsAsync(new NewUserResponse
                {
                    Username = "test@hmcts.net",
                    UserId = participant.LastName,
                    OneTimePassword = "Password"
                });
            }

            var userService = new UserService(userApiClient.Object, new Mock<ILogger<UserService>>().Object);
            var bookingsApiClient = new Mock<IBookingsApiClient>();
            var logger = new Mock<ILogger<UserCreationAndNotification>>();

            var messageHandler = (IMessageHandler)new CreateAndNotifyUserHandler(new UserCreationAndNotification(
                notificationService.Object,
                userService,
                bookingsApiClient.Object,
                logger.Object));

            await messageHandler.HandleAsync(integrationEvent);
            
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.Representative.ToString() && 
                r.GroupName == UserService.External)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.Representative.ToString() && 
                r.GroupName == UserService.VirtualRoomProfessionalUser)));
            
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.JudicialOfficeHolder.ToString() && 
                r.GroupName == UserService.External)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.JudicialOfficeHolder.ToString() && 
                r.GroupName == UserService.JudicialOfficeHolder)));
            
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.StaffMember.ToString() && 
                r.GroupName == UserService.Internal)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.StaffMember.ToString() && 
                r.GroupName == UserService.StaffMember)));
            
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                r.UserId == UserRole.Individual.ToString() && 
                r.GroupName == UserService.External)));

            foreach (var participant in integrationEvent.Participants)
            {
                userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
                    r.UserId == participant.LastName)));
            }
        }

        [Test]
        [Ignore("TODO fix")]
        public async Task should_assign_users_to_correct_group_when_request_is_valid_and_sspr_feature_is_toggled()
        {
            var notificationService = new Mock<INotificationService>();
            var userApiClient = new Mock<IUserApiClient>();
            var integrationEvent = new CreateAndNotifyUserIntegrationEvent
            {
                Hearing = GetHearingDto(),
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.Representative.ToString(),
                        UserRole = "Representative"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.JudicialOfficeHolder.ToString(),
                        UserRole = "Judicial Office Holder"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.StaffMember.ToString(),
                        UserRole = "StaffMember"
                    },
                    new ParticipantDto
                    {
                        FirstName = "Test",
                        LastName = UserRole.Individual.ToString(),
                        UserRole = "Individual"
                    }
                }
            };

            foreach (var participant in integrationEvent.Participants)
            {
                userApiClient.Setup(x => x.CreateUserAsync(It.Is<CreateUserRequest>(r => r.LastName == participant.LastName))).ReturnsAsync(new NewUserResponse
                {
                    Username = "test@hmcts.net",
                    UserId = participant.LastName,
                    OneTimePassword = "Password"
                });
            }

            var userService = new UserService(userApiClient.Object, new Mock<ILogger<UserService>>().Object);
            var bookingsApiClient = new Mock<IBookingsApiClient>();
            var logger = new Mock<ILogger<UserCreationAndNotification>>();

            var messageHandler = (IMessageHandler)new CreateAndNotifyUserHandler(new UserCreationAndNotification(
                notificationService.Object,
                userService,
                bookingsApiClient.Object,
                logger.Object));

            await messageHandler.HandleAsync(integrationEvent);

            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.Representative.ToString() &&
                r.GroupName == UserService.External)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.Representative.ToString() &&
                r.GroupName == UserService.VirtualRoomProfessionalUser)));

            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.JudicialOfficeHolder.ToString() &&
                r.GroupName == UserService.External)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.JudicialOfficeHolder.ToString() &&
                r.GroupName == UserService.JudicialOfficeHolder)));

            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.StaffMember.ToString() &&
                r.GroupName == UserService.Internal)));
            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.StaffMember.ToString() &&
                r.GroupName == UserService.StaffMember)));

            userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                r.UserId == UserRole.Individual.ToString() &&
                r.GroupName == UserService.External)));

            foreach (var participant in integrationEvent.Participants)
            {
                userApiClient.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r =>
                    r.UserId == participant.LastName &&
                    r.GroupName == UserService.SsprEnabled)), Times.Never);
            }
        }

        private CreateAndNotifyUserIntegrationEvent GetIntegrationEvent()
        {

            return new CreateAndNotifyUserIntegrationEvent
            {
                Hearing = GetHearingDto(),
                Participants = new List<ParticipantDto>
                {
                    new ParticipantDto
                    {
                        CaseGroupType = CaseRoleGroup.Applicant,
                        DisplayName = "displayName",
                        Fullname = "fullname",
                        FirstName = "firstName",
                        LastName = "lastName",
                        ContactEmail = "test@hmcts.net",
                        ContactTelephone = "012748465859",
                        HearingRole = "hearingRole",
                        ParticipantId = ParticipantId,
                        Representee = "representee",
                        UserRole = UserRole.Individual.ToString(),
                        Username = "username",
                        LinkedParticipants = new List<LinkedParticipantDto>()
                    }
                }
            };
        }

        private static HearingDto GetHearingDto()
        {
            return new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Generic",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true
            };
        }
    }
}