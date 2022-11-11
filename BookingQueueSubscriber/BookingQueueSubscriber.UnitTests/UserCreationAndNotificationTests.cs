using System;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using BookingsApi.Contract.Configuration;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests
{
    public class UserCreationAndNotificationTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IBookingsApiClient> _bookingsAPIMock;
        private Mock<ILogger<UserCreationAndNotification>> _logger;
        private Mock<ILogger<UserService>> _logger2;
        private Mock<IUserApiClient> _userApi;
        private Mock<IFeatureToggles> _featureTogglesMock;
        public UserCreationAndNotificationTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _userServiceMock = new Mock<IUserService>();
            _bookingsAPIMock = new Mock<IBookingsApiClient>();
            _logger = new Mock<ILogger<UserCreationAndNotification>>();
            _logger2 = new Mock<ILogger<UserService>>();
            _userApi = new Mock<IUserApiClient>();
            _featureTogglesMock = new Mock<IFeatureToggles>();
        }

        [Test]
        public async Task should_have_called_CreateNewUserForParticipantAsync_for_joh_when_eJudfeature_disabled()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, false);

            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object, 
                _userServiceMock.Object, _bookingsAPIMock.Object, _logger.Object);

            await userCreationAndNotification.CreateUserAndNotifcationAsync(hearing, new List<ParticipantDto>
                {
                   participant
                });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName,participant.LastName, participant.ContactEmail,false), Times.Once());
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, "xyz"), Times.Once);
        }

        [Test]
        public async Task should_not_have_called_CreateNewUserForParticipantAsync_for_joh_when_eJudfeature_enabled()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, true);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                _userServiceMock.Object, _bookingsAPIMock.Object, _logger.Object);

            await userCreationAndNotification.CreateUserAndNotifcationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, It.IsAny<String>() ),Times.Never);
        }
        
        [Test]
        public async Task should_return_created_user_CreateNewUserForParticipantAsync_for_create_user_exists_first_time()
        {
            var participant = GetParticipant();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, true);

            var user = new UserProfile()
            {
                UserId = "1",
                UserName = participant.Username
            };
            
            _userApi.SetupSequence(x => x.GetUserByEmailAsync(participant.ContactEmail))
                .ThrowsAsync(new UserApiException("Not Found",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null))
                .ReturnsAsync(user);
                            
            _featureTogglesMock.Setup(x => x.SsprToggle()).Returns(false);
            
            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.Conflict,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object, _featureTogglesMock.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object, _logger.Object);

            await userCreationAndNotification.CreateUserAndNotifcationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _userApi.Verify(x=>x.GetUserByEmailAsync(participant.ContactEmail), Times.Exactly(2));
            _bookingsAPIMock.Verify(x=>x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Once);
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, It.IsAny<String>() ),Times.Once);
        }
        
        [Test]
        public async Task should_return_null_user_CreateNewUserForParticipantAsync_for_get_user_fail_twice()
        {
            var participant = GetParticipant();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, true);
            
            _userApi.Reset();
            _bookingsAPIMock.Reset();   
            
            _userApi.Setup(x => x.GetUserByEmailAsync(participant.ContactEmail))
                .ThrowsAsync(new UserApiException("Not Found",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
                            
            _featureTogglesMock.Setup(x => x.SsprToggle()).Returns(false);
            
            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.Conflict,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object, _featureTogglesMock.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object, _logger.Object);

            await userCreationAndNotification.CreateUserAndNotifcationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _userApi.Verify(x=>x.GetUserByEmailAsync(participant.ContactEmail), Times.Exactly(2));
            _bookingsAPIMock.Verify(x=>x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Never);
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, It.IsAny<String>() ),Times.Never);
        }

        [Test]
        public async Task should_return_exception_CreateNewUserForParticipantAsync_for_create_user_Throw_exception_different_from_Conflict()
        {
            var participant = GetParticipant();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, true);

            _userApi.Reset();
            _bookingsAPIMock.Reset();
            var user = new UserProfile()
            {
                UserId = "1",
                UserName = participant.Username
            };
            
            _userApi.Setup(x => x.GetUserByEmailAsync(participant.ContactEmail))
                .ThrowsAsync(new UserApiException("Not Found",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
                            
            _featureTogglesMock.Setup(x => x.SsprToggle()).Returns(false);
            
            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object, _featureTogglesMock.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object, _logger.Object);

            await userCreationAndNotification.CreateUserAndNotifcationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _userApi.Verify(x=>x.GetUserByEmailAsync(participant.ContactEmail), Times.Once);
            _bookingsAPIMock.Verify(x=>x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Never);
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, It.IsAny<String>() ),Times.Never);
        }

        private void SetupDependencyCalls(ParticipantDto participant, HearingDto hearing, bool eJudFeatureFlag)
        {
            _bookingsAPIMock.Setup(x => x.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature))).ReturnsAsync(eJudFeatureFlag);
            _bookingsAPIMock.Setup(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username));
            _notificationServiceMock.Setup(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, "xyz"));
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail,
                false)).ReturnsAsync(new User { UserId = "part1@hearigns.reform.hmcts.net", Password = "xyz", UserName = "part1@hearigns.reform.hmcts.net" });
        }

        private static ParticipantDto GetJoh()
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
        
        private static ParticipantDto GetParticipant()
        {
            return new ParticipantDto
            {
                ContactEmail = "part1@hmcts.net",
                Username = "part1@hmcts.net",
                UserRole = "Judicial Office Holder",
                FirstName = "part1",
                LastName = "joh"
            };
        }
    }
}