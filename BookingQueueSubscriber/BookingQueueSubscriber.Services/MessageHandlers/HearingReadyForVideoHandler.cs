using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService, IVideoWebService videoWebService, 
            IUserService userService, 
            INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            await HandleUserCreationAndNotifications(eventMessage);

            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.Endpoints);

            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(request);
            
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);
        }

        private async Task HandleUserCreationAndNotifications(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            foreach (var participant in eventMessage.Participants)
            {
                await CreateUserAndSendNotificationAsync(eventMessage.Hearing.HearingId, participant);
            }

            if (!eventMessage.Hearing.GroupId.HasValue ||
                eventMessage.Hearing.GroupId.GetValueOrDefault() == Guid.Empty) // Not a multi day hearing
            {
                await _notificationService.SendNewHearingNotification(eventMessage.Hearing, eventMessage.Participants);
            }
        }

        private async Task CreateUserAndSendNotificationAsync(Guid hearingId, ParticipantDto participant)
        {
            User user = null;
            if (!string.Equals(participant.UserRole, RoleNames.Judge))
            {
                user = await _userService.CreateNewUserForParticipantAsync(participant.FirstName,
                    participant.LastName, participant.ContactEmail, false);
                participant.Username = user.UserName;
                // Update participant with the user name though bookings api.
            }

            if (user != null)
            {
                await _notificationService.SendNewUserAccountNotificationAsync(hearingId, participant, user.Password);
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent)integrationEvent);
        }
    }
}