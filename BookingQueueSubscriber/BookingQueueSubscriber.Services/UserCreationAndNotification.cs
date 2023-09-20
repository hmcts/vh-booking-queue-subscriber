using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Client;
using BookingsApi.Contract.V1.Configuration;

namespace BookingQueueSubscriber.Services
{
    public interface IUserCreationAndNotification
    {
        /// <summary>
        /// Create a new user in AD.
        /// <para>
        ///     Sends the new user a welcome email, if the participant meets the requirements. (NewUserLipWelcome)
        ///     Sends the new user their account details (username and password). (CreateIndividual, CreateRepresentative)
        /// <remarks>
        ///     If the new template toggle is on, then the new confirmation email is sent with the credentials. (NewUserHearingConfirmation)
        /// </remarks>
        /// </para>
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participants"></param>
        /// <returns>a list of newly created accounts</returns>
        Task<IList<UserDto>> CreateUserAndSendNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants);
        
        /// <summary>
        /// Add the user to the correct group in AD based on their hearing role
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Task AssignUserToGroupForHearing(IList<UserDto> users);
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

        public async Task<IList<UserDto>> CreateUserAndSendNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants)
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

        public async Task AssignUserToGroupForHearing(IList<UserDto> users)
        {
            foreach (var user in users)
            {
                _logger.LogInformation("Asign user {Username} with role {UserRole} to group", user.Username, user.UserRole);
                await _userService.AssignUserToGroup(user.UserId, user.UserRole);
            }
        }

        /// <summary>
        /// <para>Create a new users via User API that is not a panel member or winger.</para>
        /// <para>For panel members and wingers, the username is provided by the Ejud service.</para>
        /// <para>Then send a welcome email to the new user.</para>
        /// <para>Then send new account details (username and password) to the new user.</para>
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="participant"></param>
        /// <returns>a User object only if an account has been created</returns>
        private async Task<User> CreateUserAndSendNotificationAsync(HearingDto hearing, ParticipantDto participant)
        {
            User user = null;
            var ejudFeatureFlag = await _bookingsApiClient.GetFeatureFlagAsync(nameof(FeatureFlags.EJudFeature));
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

            // this will not be null when a user has been successfully created
            if (user != null)
            {
                if (_featureToggles.UsePostMay2023Template() && participant.IsIndividual())
                {
                    await _notificationService.SendNewUserWelcomeEmail(hearing, participant);
                    await _notificationService.SendNewUserSingleDayHearingConfirmationEmail(hearing, participant, user.Password);
                }
                else
                {
                    await _notificationService.SendNewUserAccountNotificationAsync(hearing.HearingId, participant, user.Password);    
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
