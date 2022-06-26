using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Enums;
using BookingsApi.Contract.Enums;

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