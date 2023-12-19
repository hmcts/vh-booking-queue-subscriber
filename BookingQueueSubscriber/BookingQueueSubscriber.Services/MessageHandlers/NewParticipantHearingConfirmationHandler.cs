using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantHearingConfirmationHandler : IMessageHandler<NewParticipantHearingConfirmationEvent>
    {
        private readonly IUserService _userService;
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoApiService _videoApiService;

        public NewParticipantHearingConfirmationHandler(IUserService userService,
            INotificationApiClient notificationApiClient,
            IBookingsApiClient bookingsApiClient,
            IVideoApiService _videoApiService)
        {
            _userService = userService;
            _notificationApiClient = notificationApiClient;
            _bookingsApiClient = bookingsApiClient;
            this._videoApiService = _videoApiService;
        }

        public async Task HandleAsync(NewParticipantHearingConfirmationEvent eventMessage)
        {
            var message = eventMessage.HearingConfirmationForParticipant;
            var newUser = await _userService.CreateNewUserForParticipantAsync(message.FirstName,
                message.LastName, message.ContactEmail, false);

            message.Username = newUser.UserName;

            
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(message.HearingId, true); 
            
            var request = new NewUserSingleDayHearingConfirmationRequest
            {
                HearingId = message.HearingId,
                ContactEmail = message.ContactEmail,
                ParticipantId = message.ParticipantId,
                CaseName = message.CaseName,
                ScheduledDateTime = message.ScheduledDateTime,
                Username = newUser.UserName,
                RoleName = message.UserRole,
                CaseNumber = message.CaseNumber,
                RandomPassword = newUser.Password,
                Name = $"{message.FirstName} {message.LastName}"
            };
            
            await _bookingsApiClient.UpdatePersonUsernameAsync(message.ContactEmail, message.Username);
            await _userService.AssignUserToGroup(newUser.UserId, message.UserRole);
            await _notificationApiClient.SendParticipantSingleDayHearingConfirmationForNewUserEmailAsync(request);
            
            var participant = conferenceResponse.Participants.Single(x => x.ContactEmail == message.ContactEmail);
            var updateParticipantDetailsRequest = new UpdateParticipantRequest
            {
                ParticipantRefId = participant.RefId,
                FirstName = message.FirstName,
                LastName = message.LastName,
                Fullname = $"{message.FirstName} {message.LastName}",
                DisplayName = eventMessage.HearingConfirmationForParticipant.DisplayName,
                Representee = message.Representee,
                ContactEmail = message.ContactEmail,
                ContactTelephone = message.ContactTelephone,
                Username = newUser.UserName
            };
            await _videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participant.Id, updateParticipantDetailsRequest);
        }
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantHearingConfirmationEvent)integrationEvent);
        }
    }
}