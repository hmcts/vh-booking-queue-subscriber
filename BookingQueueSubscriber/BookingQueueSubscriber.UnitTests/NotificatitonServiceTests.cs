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
using NotificationApi.Client;

namespace BookingQueueSubscriber.UnitTests
{
    public class NotificatitonServiceTests
    {
        private Mock<INotificationApiClient> _notificationApiMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IBookingsApiClient> _bookingsAPIMock;
        private Mock<ILogger<NotificationService>> _logger;
        public NotificatitonServiceTests()
        {
            _notificationApiMock = new Mock<INotificationApiClient>();
            _userServiceMock = new Mock<IUserService>();
            _bookingsAPIMock = new Mock<IBookingsApiClient>();
            _logger = new Mock<ILogger<NotificationService>>();
        }

        [Test]
        public async Task SendNewHearingNotification_should_have_map_to_newhearing_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic" };

            SetupDependencyCalls(false);

            var notificationService = new NotificationService(_notificationApiMock.Object, 
                _bookingsAPIMock.Object, _logger.Object);

            await notificationService.SendNewHearingNotification(hearing, new List<ParticipantDto>
                {
                   participant
                });

        }
        [Test]
        public async Task SendHearingAmendmentNotification_should_have_map_to_hearingamendment_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic", ScheduledDateTime = DateTime.UtcNow.AddDays(1) };

            SetupDependencyCalls(false);

            var notificationService = new NotificationService(_notificationApiMock.Object,
                _bookingsAPIMock.Object, _logger.Object);

            await notificationService.SendHearingAmendmentNotificationAsync(hearing, DateTime.UtcNow, new List<ParticipantDto>
                {
                   participant
                });

        }

        [Test]
        public async Task SendMultiDayHearingNotification_should_have_map_to_hearingamendment_notification_with_feature_flag()
        {
            var participant = GetJoh();
            var hearing = new HearingDto { HearingId = Guid.NewGuid(), CaseType = "Non-Generic", CaseName = "multi day test" };

            SetupDependencyCalls(false);

            var notificationService = new NotificationService(_notificationApiMock.Object,
                _bookingsAPIMock.Object, _logger.Object);

            await notificationService.SendMultiDayHearingNotificationAsync(hearing, new List<ParticipantDto>
                {
                   participant
                }, 10);

        }

        private void SetupDependencyCalls(bool eJudFeatureFlag)
        {
            _bookingsAPIMock.Setup(x => x.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature))).ReturnsAsync(eJudFeatureFlag);

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