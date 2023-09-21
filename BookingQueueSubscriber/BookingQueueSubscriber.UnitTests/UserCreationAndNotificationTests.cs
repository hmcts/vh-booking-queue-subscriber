using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using Microsoft.Extensions.Logging;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using System.Net;
using BookingQueueSubscriber.Common.Configuration;
using BookingsApi.Contract.V1.Configuration;
using UserApi.Client;
using UserApi.Contract.Requests;
using UserApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests
{
    public class UserCreationAndNotificationTests
    {
        private const string PasswordForNotification = "xyz";
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IBookingsApiClient> _bookingsAPIMock = new();
        private readonly Mock<ILogger<UserCreationAndNotification>> _logger = new();
        private readonly Mock<ILogger<UserService>> _logger2 = new();
        private readonly Mock<IUserApiClient> _userApi = new();
        private readonly Mock<IFeatureToggles> _featureToggles = new();

        [Test]
        public async Task should_have_called_CreateNewUserForParticipantAsync_for_joh_when_eJudfeature_disabled()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, false);

            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object, 
                _userServiceMock.Object, _bookingsAPIMock.Object, _logger.Object, _featureToggles.Object);

            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
                {
                   participant
                });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName,participant.LastName, participant.ContactEmail,false), Times.Once());
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, PasswordForNotification), Times.Once);
            _notificationServiceMock.Verify(x=> x.SendNewUserWelcomeEmail(It.IsAny<HearingDto>(), It.IsAny<ParticipantDto>()), Times.Never);
        }

        [Test]
        public async Task should_not_have_called_CreateNewUserForParticipantAsync_for_joh_when_eJudfeature_enabled()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, true);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                _userServiceMock.Object, _bookingsAPIMock.Object,_logger.Object, _featureToggles.Object);

            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
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
                            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.Conflict,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object,_logger.Object, _featureToggles.Object);

            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _userApi.Verify(x=>x.GetUserByEmailAsync(participant.ContactEmail), Times.Exactly(2));
            _bookingsAPIMock.Verify(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Once);
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
                            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.Conflict,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object,_logger.Object, _featureToggles.Object);

            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
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
            
            _userApi.Setup(x => x.GetUserByEmailAsync(participant.ContactEmail))
                .ThrowsAsync(new UserApiException("Not Found",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
                            
            _userApi.Setup(x=>x.CreateUserAsync(It.IsAny<CreateUserRequest>()))
                .ThrowsAsync(new UserApiException("User already exists",
                    (int) HttpStatusCode.NotFound,
                    "", new Dictionary<string, IEnumerable<string>>(), null));
            
            var userService = new UserService(_userApi.Object, _logger2.Object);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                userService, _bookingsAPIMock.Object,_logger.Object, _featureToggles.Object);

            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            _userServiceMock.Verify(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail, true), Times.Never);
            _userApi.Verify(x=>x.GetUserByEmailAsync(participant.ContactEmail), Times.Once);
            _bookingsAPIMock.Verify(x=>x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username), Times.Never);
            _notificationServiceMock.Verify(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, It.IsAny<String>() ),Times.Never);
        }

        [Test]
        public async Task should_skip_user_when_matching_participant_not_found()
        {
            var participant = GetParticipant();
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };

            SetupDependencyCalls(participant, hearing, false);
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail,
                false)).ReturnsAsync(new User { UserId = "part1@hearigns.reform.hmcts.net", Password = PasswordForNotification, UserName = "part1@hearigns.reform.hmcts.net", ContactEmail = ""});
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object,
                _userServiceMock.Object, _bookingsAPIMock.Object,_logger.Object, _featureToggles.Object);

            var users = await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            Assert.IsEmpty(users);
        }

        [Test]
        public async Task should_send_new_user_welcome_email_when_new_template_toggle_is_on()
        {
            // arrange
            var participant = new ParticipantDto
            {
                ContactEmail = "part1@ejudiciary.net",
                Username = "part1@ejudiciary.net",
                UserRole = "Individual",
                FirstName = "part1",
                LastName = "Individual"
            };
            var hearing = new HearingDto { HearingId = Guid.NewGuid() };
            SetupDependencyCalls(participant, hearing, false, true);
            
            var userCreationAndNotification = new UserCreationAndNotification(_notificationServiceMock.Object, 
                _userServiceMock.Object, _bookingsAPIMock.Object, _logger.Object, _featureToggles.Object);

            // act
            await userCreationAndNotification.CreateUserAndSendNotificationAsync(hearing, new List<ParticipantDto>
            {
                participant
            });

            // assert
  
            
            _notificationServiceMock.Verify(x=> x.SendNewUserWelcomeEmail(hearing, participant), Times.Once);
            _notificationServiceMock.Verify(x=> x.SendNewUserSingleDayHearingConfirmationEmail(hearing, participant, PasswordForNotification), Times.Once);
            
        }

        private void SetupDependencyCalls(ParticipantDto participant, HearingDto hearing, bool eJudFeatureFlag, bool newTemplatesFlag = false)
        {
            _featureToggles.Setup(x => x.UsePostMay2023Template()).Returns(newTemplatesFlag);
            _bookingsAPIMock.Setup(x => x.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature))).ReturnsAsync(eJudFeatureFlag);
            _bookingsAPIMock.Setup(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username));
            _notificationServiceMock.Setup(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, PasswordForNotification));
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail,
                false)).ReturnsAsync(new User { UserId = "part1@hearigns.reform.hmcts.net", Password = PasswordForNotification, UserName = "part1@hearigns.reform.hmcts.net", ContactEmail = participant.ContactEmail});
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