using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    internal class UserCreationAndNotificationHelper
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public UserCreationAndNotificationHelper(INotificationService notificationService, IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task HandleUserCreationAndNotificationsAsync(HearingDto hearing, IList<ParticipantDto> participants)
        {
            foreach (var participant in participants)
            {
                await CreateUserAndSendNotificationAsync(hearing.HearingId, participant);
            }

            if (!hearing.GroupId.HasValue ||
                hearing.GroupId.GetValueOrDefault() == Guid.Empty) // Not a multi day hearing
            {
                await _notificationService.SendNewHearingNotification(hearing, participants);
            }
        }

        private async Task CreateUserAndSendNotificationAsync(Guid hearingId, ParticipantDto participant)
        {
            User user = null;
            if (!string.Equals(participant.HearingRole, RoleNames.Judge) ||
                !IsPanelMemberOrWingerWithUsername(participant))
            {
                user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                    participant.LastName, participant.ContactEmail, false);
                participant.Username = user.UserName;
                // Update participant with the user name through bookings api.
            }

            if (user != null)
            {
                await _notificationService.SendNewUserAccountNotificationAsync(hearingId, participant, user.Password);
            }
        }

        private static bool IsPanelMemberOrWingerWithUsername(ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && (string.Equals(participant.HearingRole, RoleNames.PanelMember) ||
                    string.Equals(participant.HearingRole, RoleNames.Winger));
        }
    }
}
