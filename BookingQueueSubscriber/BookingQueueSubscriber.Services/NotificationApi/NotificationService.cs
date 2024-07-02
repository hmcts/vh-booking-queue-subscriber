using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.UserApi;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using RoleNames = BookingQueueSubscriber.Services.UserApi.RoleNames;

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
        Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string userPassword);

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
        /// Send a single day hearing confirmation to new users. Part of the 2nd of 3 new template
        /// This is a combination of account details and hearing details
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task SendNewUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string userPassword);

        /// <summary>
        /// Send a single day hearing confirmation to existing users. Part of the 2nd of 3 new template
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task SendExistingUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant);

        /// <summary>
        /// Send a multi-day hearing confirmation to new and existing users. Part of the 2nd of 3 new template
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days);

    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly ILogger<NotificationService> _logger;
        private readonly IUserService _userService;

        public NotificationService(INotificationApiClient notificationApiClient, ILogger<NotificationService> logger, IUserService userService)
        {
            _notificationApiClient = notificationApiClient;
            _logger = logger;
            _userService = userService;
        }

        public async Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string userPassword)
        {
            if (!string.IsNullOrEmpty(userPassword))
            {
                _logger.LogInformation("Creating Notification for new user {username}", participant.Username);
                var request = AddNotificationRequestMapper.MapToNewUserNotification(hearingId, participant, userPassword);
                await _notificationApiClient.CreateNewNotificationAsync(request);
            }
        }

        public async Task SendNewSingleDayHearingConfirmationNotification(HearingDto hearing,
            IEnumerable<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants);
                return;
            }
            
            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant))
                .ToList();
            await CreateNotifications(requests);
            _logger.LogInformation("Created hearing notification for the users in the hearing {hearingid}",
                hearing.HearingId);
        }

        public async Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                return;
            }

            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, originalDateTime, hearing.ScheduledDateTime))
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

        public Task SendNewUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant, string userPassword)
        {
            if (hearing.IsGenericHearing())
            {
                return Task.CompletedTask;
            }
            var request = AddNotificationRequestMapper.MapToNewUserAccountDetailsEmail(hearing, participant, userPassword);
            return _notificationApiClient.CreateNewNotificationAsync(request);
        }

        public Task SendExistingUserSingleDayHearingConfirmationEmail(HearingDto hearing, ParticipantDto participant)
        {
            if (hearing.IsGenericHearing())
            {
                return Task.CompletedTask;
            }
            var request = AddNotificationRequestMapper.MapToNewUserAccountDetailsEmail(hearing, participant);
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
                    list.Add(AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days));
                }

                if (user != null)
                {
                    var userPassword = user.Password;
                    list.Add(AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days, userPassword));
                    if (!string.IsNullOrEmpty(userPassword))
                    {
                        await SendNewUserWelcomeEmail(hearing, participant);
                    }
                }
            }

            return list;
        }

        private async Task ProcessGenericEmail(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            if (string.Equals(hearing.HearingType, "Automated Test", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var notificationRequests = participants
                .Select(participant => AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, hearing.HearingType))
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
