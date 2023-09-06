﻿using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using Microsoft.Extensions.Logging;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingsApi.Contract.V1.Configuration;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests
{
    public class NotificationServiceTests
    {
        private Mock<INotificationApiClient> _notificationApiMock;
        private Mock<IBookingsApiClient> _bookingsApiMock;
        private Mock<ILogger<NotificationService>> _logger;
        private Mock<IFeatureToggles> _featureToggles;
        private Mock<IUserService> _userService;

        private NotificationService _notificationService;

        [SetUp]
        public void TestSetup()
        {
            _notificationApiMock = new Mock<INotificationApiClient>();
            _bookingsApiMock = new Mock<IBookingsApiClient>();
            _logger = new Mock<ILogger<NotificationService>>();
            _featureToggles = new Mock<IFeatureToggles>();
            _userService = new Mock<IUserService>();
            
            _notificationService = new NotificationService(_notificationApiMock.Object, 
                _bookingsApiMock.Object, _logger.Object, _featureToggles.Object, _userService.Object);
        }

        [Test]
        public async Task SendNewHearingNotification_should_have_map_to_newhearing_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic" };


            await _notificationService.SendNewHearingNotification(hearing, new List<ParticipantDto>
                {
                   participant
                });

            _bookingsApiMock.Verify(x => x.GetFeatureFlagAsync(It.IsAny<String>()), Times.Once);
        }

        [Test]
        public async Task SendHearingAmendmentNotification_should_have_map_to_hearingamendment_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic", ScheduledDateTime = DateTime.UtcNow.AddDays(1) };
            
            await _notificationService.SendHearingAmendmentNotificationAsync(hearing, DateTime.UtcNow, new List<ParticipantDto>
                {
                   participant
                });

            _bookingsApiMock.Verify(x => x.GetFeatureFlagAsync(It.IsAny<String>()), Times.Once);
        }

        [Test]
        public async Task SendMultiDayHearingNotification_should_have_map_to_hearingamendment_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic", CaseName = "multi day test" };
            
            await _notificationService.SendMultiDayHearingNotificationAsync(hearing, new List<ParticipantDto>
                {
                   participant
                }, 10);

            _bookingsApiMock.Verify(x => x.GetFeatureFlagAsync(It.IsAny<String>()), Times.Once);
        }

        [Test]
        public async Task SendNewUserWelcomeEmail_should_map_to_welcome_email()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Civil Money Claims", CaseName = "Hearing for Civil Money Claims"
            }; 
            
            var participant = new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                ContactEmail = "part1@ejudiciary.net",
                Username = "part1@ejudiciary.net",
                UserRole = "Individual",
                FirstName = "part1",
                LastName = "Individual"
            };
            
            // act
            await _notificationService.SendNewUserWelcomeEmail(hearing, participant);
            
            // assert
            _notificationApiMock.Verify(
                x => x.CreateNewNotificationAsync(
                    It.Is<AddNotificationRequest>(request =>
                        request.HearingId == hearing.HearingId &&
                        request.NotificationType == NotificationType.NewUserLipWelcome &&
                        request.ContactEmail == participant.ContactEmail &&
                        request.MessageType == MessageType.Email
                    )
                )
                , Times.Once);
        }
        
        [Test]
        public async Task SendNewUserAccountDetailsEmail_should_map_to_account_details_email()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Civil Money Claims", CaseName = "Hearing for Civil Money Claims"
            }; 
            
            var participant = new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                ContactEmail = "part1@ejudiciary.net",
                Username = "part1@ejudiciary.net",
                UserRole = "Individual",
                FirstName = "part1",
                LastName = "Individual"
            };
            
            // act
            await _notificationService.SendNewUserAccountDetailsEmail(hearing, participant, "xyz");
            
            // assert
            _notificationApiMock.Verify(
                x => x.CreateNewNotificationAsync(
                    It.Is<AddNotificationRequest>(request =>
                        request.HearingId == hearing.HearingId &&
                        request.NotificationType == NotificationType.NewUserLipConfirmation &&
                        request.ContactEmail == participant.ContactEmail &&
                        request.MessageType == MessageType.Email
                    )
                )
                , Times.Once);
        }
        
        [Test]
        public async Task SendExistingUserAccountDetailsEmail_should_map_to_account_details_email()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Civil Money Claims", CaseName = "Hearing for Civil Money Claims"
            }; 
            
            var participant = new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                ContactEmail = "part1@ejudiciary.net",
                Username = "part1@ejudiciary.net",
                UserRole = "Individual",
                FirstName = "part1",
                LastName = "Individual"
            };
            
            // act
            await _notificationService.SendExistingUserAccountDetailsEmail(hearing, participant);
            
            // assert
            _notificationApiMock.Verify(
                x => x.CreateNewNotificationAsync(
                    It.Is<AddNotificationRequest>(request =>
                        request.HearingId == hearing.HearingId &&
                        request.NotificationType == NotificationType.ExistingUserLipConfirmation &&
                        request.ContactEmail == participant.ContactEmail &&
                        request.MessageType == MessageType.Email
                    )
                )
                , Times.Once);
        }
        
        private ParticipantDto GetJoh()
        {
            return new ParticipantDto
            {
                ContactEmail = "part1@ejudiciary.net",
                Username = "part1@ejudiciary.net",
                UserRole = "Judicial Office Holder",
                FirstName = "part1",
                LastName = "joh"
            };
        }
    }
}