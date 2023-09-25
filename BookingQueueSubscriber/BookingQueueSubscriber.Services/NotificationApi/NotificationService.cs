using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using BookingsApi.Contract.V1.Configuration;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using UserApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
        Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password);
        Task SendNewHearingNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants);
        Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants);
        Task SendNewUserWelcomeEmail(HearingDto hearing, ParticipantDto participant);
        
        Task SendNewUserAccountDetailsEmail(HearingDto hearing, ParticipantDto participant, string userPassword);

        Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days);
        Task SendExistingUserAccountDetailsEmail(HearingDto hearing, ParticipantDto participant);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<NotificationService> _logger;
        private readonly IFeatureToggles _featureToggles;
        private readonly IUserService _userService;

        public NotificationService(INotificationApiClient notificationApiClient, IBookingsApiClient bookingsApiClient, ILogger<NotificationService> logger, IFeatureToggles featureToggles, IUserService userService)
        {
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
            _featureToggles = featureToggles;
            _userService = userService;
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
        public async Task SendNewHearingNotification(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants);
                return;
            }
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant, ejudFeatureFlag))
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

        public Task SendNewUserAccountDetailsEmail(HearingDto hearing, ParticipantDto participant, string userPassword)
        {
            if (hearing.IsGenericHearing())
            {
                return Task.CompletedTask;
            }
            var request = AddNotificationRequestMapper.MapToNewUserAccountDetailsEmail(hearing, participant, userPassword);
            return _notificationApiClient.CreateNewNotificationAsync(request);
        }

        public async Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants); 
                return;
            }

            List<AddNotificationRequest> requests = await CreateUserRequest(hearing, participants, days); 
           
            await CreateNotifications(requests);
        }

        private async Task<List<AddNotificationRequest>> CreateUserRequest(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            List<AddNotificationRequest> list = new List<AddNotificationRequest>();
            
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
            var usePostMay2023Template = _featureToggles.UsePostMay2023Template();
            
            foreach (var participant in participants)
            {
                User user = null;
                if (!string.Equals(participant.UserRole, RoleNames.Judge))
                {
                    user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                        participant.LastName, participant.ContactEmail, false);
                }
                else
                {
                    list.Add(AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days, ejudFeatureFlag, usePostMay2023Template));
                }
                if (user != null)
                {
                    var userPassword = user.Password;
                    list.Add(AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days, ejudFeatureFlag, usePostMay2023Template, userPassword));
                    if (!string.IsNullOrEmpty(userPassword))
                    {
                        await SendNewUserWelcomeEmail(hearing, participant);
                    }
                }
            }

            return list;
        }

        public Task SendExistingUserAccountDetailsEmail(HearingDto hearing, ParticipantDto participant)
        {
            if (hearing.IsGenericHearing())
            {
                return Task.CompletedTask;
            }
            var request = AddNotificationRequestMapper.MapToNewUserAccountDetailsEmail(hearing, participant);
            return _notificationApiClient.CreateNewNotificationAsync(request);
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
