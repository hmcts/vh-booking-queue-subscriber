using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;

namespace BookingQueueSubscriber.Services
{
    public interface IUserCreationAndNotification
    {
        Task<IList<UserDto>> CreateUserAndNotifcationAsync(HearingDto hearing, IList<ParticipantDto> participants);
        Task HandleAssignUserToGroup(IList<UserDto> users);
        Task SendHearingNotificationAsync(HearingDto hearing, IEnumerable<ParticipantDto> participants);
    }

    public class UserCreationAndNotification : IUserCreationAndNotification
    {
        private readonly INotificationService _notificationService;
        private readonly IFeatureToggles _featureToggles;
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<UserCreationAndNotification> _logger;

        public UserCreationAndNotification(INotificationService notificationService, IUserService userService, IBookingsApiClient bookingsApiClient,
             ILogger<UserCreationAndNotification> logger, IFeatureToggles featureToggles)
        {
            _notificationService = notificationService;
            _userService = userService;
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
            _featureToggles = featureToggles;
        }

        public async Task<IList<UserDto>> CreateUserAndNotifcationAsync(HearingDto hearing, IList<ParticipantDto> participants)
        {
            var createUserTasks = new List<Task<User>>();
            foreach (var participant in participants)
            {
                var task = CreateUserAndSendNotificationAsync(hearing, participant);
                createUserTasks.Add(task);
            }
            
            var users = await Task.WhenAll(createUserTasks);
            var newUsers = new List<UserDto>();

            foreach (var user in users)
            {
                if (user == null) continue;
            
                var participant = participants.FirstOrDefault(p => p.ContactEmail == user.ContactEmail);
                
                if (!string.IsNullOrEmpty(user.UserName) && participant != null) 
                {
                    newUsers.Add(new UserDto { UserId = user.UserId, Username = user.UserName, UserRole = participant.UserRole });
                }
            }

            return newUsers;

        }

        public async Task SendHearingNotificationAsync(HearingDto hearing, IEnumerable<ParticipantDto> participants)
        {
            await _notificationService.SendNewSingleDayHearingConfirmationNotification(hearing, participants);
        }

        public async Task HandleAssignUserToGroup(IList<UserDto> users)
        {
            foreach (var user in users)
            {
                _logger.LogInformation("Asign user {Username} with role {UserRole} to group", user.Username, user.UserRole);
                await _userService.AssignUserToGroup(user.UserId, user.UserRole);
            }
        }

        private async Task<User> CreateUserAndSendNotificationAsync(HearingDto hearing, ParticipantDto participant)
        {
            User user = null;
            var ejudFeatureFlag = _featureToggles.EjudFeatureToggle();
            if (!string.Equals(participant.UserRole, RoleNames.Judge) &&
                !IsPanelMemberOrWingerWithEJudUsername(participant, ejudFeatureFlag))
            {
                user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                    participant.LastName, participant.ContactEmail, false);
                if (user != null)
                {
                    participant.Username = user.UserName;
                    // Update participant with the user name through bookings api.
                    await _bookingsApiClient.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username);
                }
            }
            

            if (user != null)
            {
                var userPassword = user.Password;
                if (_featureToggles.UsePostMay2023Template() && participant.IsIndividual())
                {
                    if (!string.IsNullOrEmpty(userPassword))
                    {
                        await _notificationService.SendNewUserWelcomeEmail(hearing, participant);
                        await _notificationService.SendNewUserSingleDayHearingConfirmationEmail(hearing, participant, userPassword);
                    }
                    else
                    {
                        await _notificationService.SendExistingUserSingleDayHearingConfirmationEmail(hearing, participant);
                    }
                    
                }
                else
                {
                    await _notificationService.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, userPassword);
                }
            }

            return user;
        }

        private static bool IsPanelMemberOrWingerWithEJudUsername(ParticipantDto participant, bool ejudFeatureFlag)
        {
            if (string.Equals(participant.UserRole, RoleNames.JudicialOfficeHolder) && ejudFeatureFlag)
            {
                return participant.HasEjdUsername();
            }

            return false;
        }
    }
}
