using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using NotificationApi.Client;
using NotificationApi.Contract;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests
{
    public class NotificationServiceTests
    {
        private Mock<INotificationApiClient> _notificationApiMock;
        private Mock<ILogger<NotificationService>> _logger;
        private Mock<IUserService> _userService;

        private NotificationService _notificationService;

        [SetUp]
        public void TestSetup()
        {
            _notificationApiMock = new Mock<INotificationApiClient>();
            _logger = new Mock<ILogger<NotificationService>>();
            _userService = new Mock<IUserService>();
            
            _notificationService = new NotificationService(_notificationApiMock.Object, _logger.Object, _userService.Object);
        }

        [Test]
        public async Task SendNewHearingNotification_should_have_map_to_newhearing_notification_with_feature_flag_not_generic_hearing()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic" };


            await _notificationService.SendNewSingleDayHearingConfirmationNotification(hearing, new List<ParticipantDto>
            {
                participant
            });
            
            _notificationApiMock.Verify(x=>x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Once);

        }
        
        [Test]
        public async Task SendNewHearingNotification_should_have_map_to_newhearing_notification_with_feature_flag_generic_hearing()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Generic" };


            await _notificationService.SendNewSingleDayHearingConfirmationNotification(hearing, new List<ParticipantDto>
            {
                participant
            });
            
            _notificationApiMock.Verify(x=>x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Once);

        }
        
        [Test]
        public async Task SendNewUserAccountNotification_should_map_to_newUser_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic" };


            await _notificationService.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, "myPassword" );
            
            _notificationApiMock.Verify(x=>x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Once);

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

            _notificationApiMock.Verify(x=>x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Once);
        }
        
        [Test]
        public async Task SendHearingAmendmentNotification_should_have_map_to_hearingamendment_notification_with_feature_flag_generic()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Generic", ScheduledDateTime = DateTime.UtcNow.AddDays(1) };
            
            await _notificationService.SendHearingAmendmentNotificationAsync(hearing, DateTime.UtcNow, new List<ParticipantDto>
            {
                participant
            });

            _notificationApiMock.Verify(x=>x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Never);
        }

        [Test]
        public async Task SendMultiDayHearingNotification_should_have_map_to_hearingamendment_notification_with_feature_flag()
        {
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic", CaseName = "multi day test" };
            _userService.Setup(us=> us.CreateNewUserForParticipantAsync("part2","individual","part2@ejudiciary.net",It.IsAny<bool>()))
                .ReturnsAsync(new Func<User>(() => new User(){ ContactEmail = "part2@ejudiciary.net", Password = "MyPassword", UserName = "part2@ejudiciary.net"}));
            
            await _notificationService.SendMultiDayHearingNotificationAsync(hearing, CreateParticipants(), 10);

            _notificationApiMock.Verify(x => x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.AtLeastOnce);
        }
        
        [Test]
        public async Task SendMultiDayHearingNotification_should_have_map_to_hearingamendment_notification_with_feature_flag_generic()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Generic", CaseName = "multi day test" };
            
            await _notificationService.SendMultiDayHearingNotificationAsync(hearing, new List<ParticipantDto>
            {
                participant
            }, 10);

            _notificationApiMock.Verify(x => x.CreateNewNotificationAsync(It.IsAny<AddNotificationRequest>()), Times.Once);
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
        public async Task SendNewUserWelcomeEmail_should_map_to_welcome_email_generic()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Generic", CaseName = "Hearing for Civil Money Claims"
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
                        request.NotificationType == NotificationType.ParticipantDemoOrTest &&
                        request.ContactEmail == participant.ContactEmail &&
                        request.MessageType == MessageType.Email
                    )
                )
                , Times.Never);
        }
        
        [Test]
        public async Task SendNewUserSingleDayHearingConfirmationEmail_should_map_to_account_details_email()
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
            await _notificationService.SendNewUserSingleDayHearingConfirmationEmail(hearing, participant, "xyz");
            
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
        public async Task SendNewUserSingleDayHearingConfirmationEmail_should_map_to_account_details_email_generic()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Generic", CaseName = "Hearing for Civil Money Claims"
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
            await _notificationService.SendNewUserSingleDayHearingConfirmationEmail(hearing, participant, "xyz");
            
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
                , Times.Never);
        }
        
        [Test]
        public async Task SendExistingUserSingleDayHearingConfirmationEmail_should_map_to_account_details_email()
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
            await _notificationService.SendExistingUserSingleDayHearingConfirmationEmail(hearing, participant);
            
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
        
        [Test]
        public async Task SendExistingUserSingleDayHearingConfirmationEmail_should_map_to_account_details_email_generic()
        {
            // arrange
            var hearing = new HearingDto
            {
                HearingId = Guid.NewGuid(), CaseType = "Generic", CaseName = "Hearing for Civil Money Claims"
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
            await _notificationService.SendExistingUserSingleDayHearingConfirmationEmail(hearing, participant);
            
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
                , Times.Never);
        }

        private static List<ParticipantDto> CreateParticipants()
        {
            List<ParticipantDto> list = new List<ParticipantDto>();
            
            list.Add(GetJoh());
            list.Add(GetJudge());
            list.Add(GetIndividual());
            
            
            return list;
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
        
        private static ParticipantDto GetIndividual()
        {
            return new ParticipantDto
            {
                ContactEmail = "part2@ejudiciary.net",
                Username = "part2@ejudiciary.net",
                UserRole = "Individual",
                FirstName = "part2",
                LastName = "individual"
            };
        }
        
        private static ParticipantDto GetJudge()
        {
            return new ParticipantDto
            {
                ContactEmail = "judge@ejudiciary.net",
                Username = "judge@ejudiciary.net",
                UserRole = "Judge",
                FirstName = "part1",
                LastName = "Judge"
            };
        }
    }
}