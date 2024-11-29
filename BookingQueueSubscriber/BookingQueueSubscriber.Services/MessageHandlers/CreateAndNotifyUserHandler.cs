using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateAndNotifyUserHandler : IMessageHandler<CreateAndNotifyUserIntegrationEvent>
    {
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public CreateAndNotifyUserHandler(IUserService userService,
            INotificationApiClient notificationApiClient,
            IBookingsApiClient bookingsApiClient,
            IVideoApiService videoApiService,
            IVideoWebService videoWebService)
        {
            _userService = userService;
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(CreateAndNotifyUserIntegrationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                    message.LastName, message.ContactEmail, false);

            message.Username = newUser.UserName;
            
            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
            await _videoApiService.UpdateParticipantUsernameWithPolling(message.HearingId, newUser.UserName, message.ContactEmail);
            await _notificationApiClient.SendParticipantCreatedAccountEmailAsync(new SignInDetailsEmailRequest
            {
                ContactEmail = message.ContactEmail,
                Name = $"{message.FirstName} {message.LastName}",
                RoleName = message.UserRole,
                Username = message.Username,
                Password = newUser.Password,
            });
            var conference = await _videoApiService.GetConferenceByHearingRefId(message.HearingId);
            await _videoWebService.PushParticipantsUpdatedMessage(conference.Id, new UpdateConferenceParticipantsRequest());
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateAndNotifyUserIntegrationEvent)integrationEvent);
        }
    }
}