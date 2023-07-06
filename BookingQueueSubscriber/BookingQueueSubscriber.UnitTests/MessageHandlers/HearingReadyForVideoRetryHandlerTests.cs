using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using UserApi.Contract.Responses;
using VideoApi.Contract.Enums;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingReadyForVideoRetryHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_create_conference_and_notify_async_when_users_do_not_exist()
        {
            var messageHandler = new HearingReadyForVideoRetryHandler(UserServiceMock.Object, ConferenceCreationAndNotificationMock.Object );
            
            var integrationEvent = CreateEvent();
            UserServiceMock.Setup(x => x.GetUserByContactEmail(It.IsAny<string>())).ReturnsAsync((UserProfile)null);
            await messageHandler.HandleAsync(integrationEvent);
            
            ConferenceCreationAndNotificationMock.Verify(x => x.CreateConferenceAndNotifyAsync(It.Is<CreateConferenceAndNotifyRequest>(
                    r => r.Hearing == integrationEvent.Hearing &&
                         r.ParticipantUsersToCreate.Count == integrationEvent.Participants.Count &&
                         r.Participants == integrationEvent.Participants &&
                         r.Endpoints == integrationEvent.Endpoints)), 
                Times.Once);
        }
        
        [Test]
        public async Task should_call_create_conference_and_notify_async_when_users_exist()
        {
            var messageHandler = new HearingReadyForVideoRetryHandler(UserServiceMock.Object, ConferenceCreationAndNotificationMock.Object );
            
            var integrationEvent = CreateEvent();
            foreach (var participant in integrationEvent.Participants)
            {
                UserServiceMock.Setup(x => x.GetUserByContactEmail(participant.ContactEmail)).ReturnsAsync(new UserProfile
                {
                    UserName = participant.Username, 
                    Email = participant.ContactEmail
                });
            }
            await messageHandler.HandleAsync(integrationEvent);
            
            ConferenceCreationAndNotificationMock.Verify(x => x.CreateConferenceAndNotifyAsync(It.Is<CreateConferenceAndNotifyRequest>(
                    r => r.Hearing == integrationEvent.Hearing &&
                         r.ParticipantUsersToCreate.Count == 0 &&
                         r.Participants == integrationEvent.Participants &&
                         r.Endpoints == integrationEvent.Endpoints)), 
                Times.Once);
        }
        
        [Test]
        public async Task should_call_create_conference_and_notify_async_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new HearingReadyForVideoRetryHandler(UserServiceMock.Object, ConferenceCreationAndNotificationMock.Object);
            
            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            
            ConferenceCreationAndNotificationMock.Verify(x => x.CreateConferenceAndNotifyAsync(It.Is<CreateConferenceAndNotifyRequest>(
                    r => r.Hearing == integrationEvent.Hearing &&
                         r.ParticipantUsersToCreate.Count == integrationEvent.Participants.Count &&
                         r.Participants == integrationEvent.Participants &&
                         r.Endpoints == integrationEvent.Endpoints)), 
                Times.Once);
        }

        private static HearingIsReadyForVideoRetryIntegrationEvent CreateEvent()
        {
            var hearingDto = new HearingDto
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
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build().ToList();

            var endpoints = Builder<EndpointDto>.CreateListOfSize(4).Build().ToList();
            
            var message = new HearingIsReadyForVideoRetryIntegrationEvent
            {
                Hearing = hearingDto,
                Participants = participants,
                Endpoints = endpoints
            };
            return message;
        }
    }
}
