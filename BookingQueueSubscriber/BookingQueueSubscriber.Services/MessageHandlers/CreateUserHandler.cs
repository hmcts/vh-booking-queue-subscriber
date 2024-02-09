using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingsApi.Client;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class CreateUserHandler : IMessageHandler<CreateUserIntegrationEvent>
    {
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoApiService _videoApiService;
        
        public CreateUserHandler(IUserService userService,
            IBookingsApiClient bookingsApiClient,
            IVideoApiService videoApiService)
        {
            _userService = userService;
            _bookingsApiClient = bookingsApiClient;
            _videoApiService = videoApiService;
        }
        
        public async Task HandleAsync(CreateUserIntegrationEvent eventMessage)
        {
            var message = eventMessage.Participant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                message.LastName, message.ContactEmail, false);

            message.Username = newUser.UserName;
            
            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
            await _videoApiService.UpdateParticipantUsernameWithPolling(message.HearingId, newUser.UserName, message.ContactEmail);
        }
        
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((CreateUserIntegrationEvent)integrationEvent);
        }
    }
}
