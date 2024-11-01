using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests
{
    public class NotificationServiceTests
    {
        private Mock<INotificationApiClient> _notificationApiMock;

        private NotificationService _notificationService;

        [SetUp]
        public void TestSetup()
        {
            _notificationApiMock = new Mock<INotificationApiClient>();
            
            _notificationService = new NotificationService(_notificationApiMock.Object);
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
    }
}