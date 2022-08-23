using System;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Contract.Configuration;

namespace BookingQueueSubscriber.UnitTests
{
    public class UserCreationAndNotificationTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IBookingsApiClient> _bookingsAPIMock;
        private Mock<ILogger<UserCreationAndNotification>> _logger;
        public UserCreationAndNotificationTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _userServiceMock = new Mock<IUserService>();
            _bookingsAPIMock = new Mock<IBookingsApiClient>();
            _logger = new Mock<ILogger<UserCreationAndNotification>>();
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

        private void SetupDependencyCalls(ParticipantDto participant, HearingDto hearing, bool eJudFeatureFlag)
        {
            _bookingsAPIMock.Setup(x => x.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature))).ReturnsAsync(eJudFeatureFlag);
            _bookingsAPIMock.Setup(x => x.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username));
            _notificationServiceMock.Setup(x => x.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, "xyz"));
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(participant.FirstName, participant.LastName, participant.ContactEmail,
                false)).ReturnsAsync(new User { UserId = "part1@hearigns.reform.hmcts.net", Password = "xyz", UserName = "part1@hearigns.reform.hmcts.net" });
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