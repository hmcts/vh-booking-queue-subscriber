using BookingsApi.Client;
using BookingsApi.Contract.V1.Configuration;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
        /// <summary>
        /// This sends a notification to the participant with their username and password
        /// <list type="bullet">
        ///     <item>NotificationType.CreateIndividual</item>
        ///     <item>NotificationType.CreateRepresentative</item> 
        /// </list>
        /// </summary>
        /// <param name="hearingId"></param>
        /// <param name="participant"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password);
        
        /// <summary>
        /// This send the hearing confirmation notification for a single day hearing
        /// <list type="bullet">
        /// <item>HearingConfirmationLip</item>
        /// <item>HearingConfirmationRepresentative</item>
        /// <item>HearingConfirmationJudge</item>
        /// <item>HearingConfirmationJoh</item>
        /// </list>
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        Task SendNewSingleDayHearingConfirmationNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants);
        
        /// <summary>
        /// Send the hearing amendment notification for a single hearing
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="originalDateTime"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants);
        
        /// <summary>
        /// Send a welcome to VH email to new users. Part of the 1st of 3 new template
        /// <remarks>NOTE: Do not send to existing users</remarks>
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant);
        
        /// <summary>
        /// Send the hearing confirmation notification for a multi day hearing
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participants"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        Task SendNewMultiDayHearingConfirmationNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationApiClient notificationApiClient, IBookingsApiClient bookingsApiClient, ILogger<NotificationService> logger)
        {
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
        }

        public async Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                _logger.LogInformation("Creating Notification for new user {username}", participant.Username);
                var request = AddNotificationRequestMapper.MapToNewUserNotification(hearingId, participant, password);
                await _notificationApiClient.CreateNewNotificationAsync(request);
            }
        }
        public async Task SendNewSingleDayHearingConfirmationNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants);
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToNewHearingConfirmationNotification(hearing, participant, ejudFeatureFlag))
                .ToList();
            await CreateNotifications(requests);
            _logger.LogInformation("Created hearing notification for the users in the hearing {hearingid}", hearing.HearingId);
        }

        public async Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                return;
            }

            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant,
                        originalDateTime, hearing.ScheduledDateTime, ejudFeatureFlag))
                .ToList();

            await CreateNotifications(requests);
        }

        public Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant)
        {
            if (hearing.IsGenericHearing())
            {
                return Task.CompletedTask;
            }
            var request = AddNotificationRequestMapper.MapToNewUserWelcomeEmail(hearing, participant);
            return _notificationApiClient.CreateNewNotificationAsync(request);
        }

        public async Task SendNewMultiDayHearingConfirmationNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants); 
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days, ejudFeatureFlag))
                .ToList();

              await CreateNotifications(requests);
        }

        private async Task ProcessGenericEmail(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (string.Equals(hearing.HearingType, "Automated Test", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var notificationRequests = participants
                .Select(participant => AddNotificationRequestMapper.MapToDemoOrTestNotification(
                    hearing, participant, hearing.HearingType, ejudFeatureFlag))
                .Where(x => x != null)
                .ToList();

            if (!notificationRequests.Any()) return;
            await CreateNotifications(notificationRequests);
        }

        private async Task CreateNotifications(List<AddNotificationRequest> notificationRequests)
        {
            notificationRequests = notificationRequests.Where(req => !string.IsNullOrWhiteSpace(req.ContactEmail)).ToList();
            await Task.WhenAll(notificationRequests.Select(_notificationApiClient.CreateNewNotificationAsync));
        }
    }
}
