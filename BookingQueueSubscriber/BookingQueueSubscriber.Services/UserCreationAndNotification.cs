using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using BookingsApi.Client;

namespace BookingQueueSubscriber.Services
{
    public interface IUserCreationAndNotification
    {
        Task<IList<UserDto>> HandleUserCreationAndNotificationsAsync(HearingDto hearing, IList<ParticipantDto> participants);
        Task HandleAssignUserToGroup(IList<UserDto> users);
    }

    public class UserCreationAndNotification : IUserCreationAndNotification
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly ILogger<UserCreationAndNotification> _logger;

        public UserCreationAndNotification(INotificationService notificationService, IUserService userService, IBookingsApiClient bookingsApiClient,
             ILogger<UserCreationAndNotification> logger)
        {
            _notificationService = notificationService;
            _userService = userService;
            _bookingsApiClient = bookingsApiClient;
            _logger = logger;
        }

        public async Task<IList<UserDto>> HandleUserCreationAndNotificationsAsync(HearingDto hearing, IList<ParticipantDto> participants)
        {
            var newUsers = new List<UserDto>();
            foreach (var participant in participants)
            {
                var user = await CreateUserAndSendNotificationAsync(hearing.HearingId, participant);
                if (!string.IsNullOrEmpty(user?.UserName))
                {
                    newUsers.Add(new UserDto { UserId = user.UserId, Username = user.UserName, UserRole = participant.UserRole});
                }
            }

            if (!hearing.GroupId.HasValue ||
                hearing.GroupId.GetValueOrDefault() == Guid.Empty) // Not a multi day hearing
            {
                await _notificationService.SendNewHearingNotification(hearing, participants);
            }

            return newUsers;
        }

        public async Task HandleAssignUserToGroup(IList<UserDto> users)
        {
            foreach (var user in users)
            {
                await _userService.AssignUserToGroup(user.UserId, user.UserRole);
            }
        }

        private async Task<User> CreateUserAndSendNotificationAsync(Guid hearingId, ParticipantDto participant)
        {
            User user = null;
            if (!string.Equals(participant.HearingRole, RoleNames.Judge) &&
                !IsPanelMemberOrWingerWithUsername(participant))
            {
                user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                    participant.LastName, participant.ContactEmail, false);
                participant.Username = user.UserName;
                // Update participant with the user name through bookings api.
                await _bookingsApiClient.UpdatePersonUsernameAsync(participant.ContactEmail, participant.Username);
            }

            if (user != null)
            {
                await _notificationService.SendNewUserAccountNotificationAsync(hearingId, participant, user.Password);
            }

            return user;
        }

        private static bool IsPanelMemberOrWingerWithUsername(ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && 
                (string.Equals(participant.HearingRole, RoleNames.JudicialOfficeHolder)) && 
                participant.HasEjdUsername();
        }
    }
}
