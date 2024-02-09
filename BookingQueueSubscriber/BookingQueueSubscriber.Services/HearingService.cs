using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingsApi.Client;

namespace BookingQueueSubscriber.Services
{
    public interface IHearingService
    {
        Task<User> CreateUserForHearing(Guid hearingId, string firstName, string lastName, string contactEmail, string userRole);
    }
    
    public class HearingService : IHearingService
    {
        private readonly IUserService _userService;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoApiService _videoApiService;

        public HearingService(IUserService userService,
            IBookingsApiClient bookingsApiClient,
            IVideoApiService videoApiService)
        {
            _userService = userService;
            _bookingsApiClient = bookingsApiClient;
            _videoApiService = videoApiService;
        }
        
        public async Task<User> CreateUserForHearing(Guid hearingId, 
            string firstName, 
            string lastName, 
            string contactEmail, 
            string userRole)
        {
            var newUser = await _userService.CreateNewUserForParticipantAsync(firstName,
                lastName, contactEmail, false);
          
            await _bookingsApiClient.UpdatePersonUsernameAsync(contactEmail, newUser.UserName);
            await _userService.AssignUserToGroup(newUser.UserId, userRole);
            await _videoApiService.UpdateParticipantUsernameWithPolling(hearingId, newUser.UserName, contactEmail);

            return newUser;
        }
    }
}
