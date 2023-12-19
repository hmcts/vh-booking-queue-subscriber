using System.Net;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingsApi.Client;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using VideoApi.Client;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class NewParticipantHearingConfirmationHandler : IMessageHandler<NewParticipantHearingConfirmationEvent>
    {
        private readonly IUserService _userService;
        private readonly INotificationApiClient _notificationApiClient;
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IVideoApiService _videoApiService;
        int pollCount = 0;

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
            ConferenceDetailsResponse conferenceResponse;
            do {
                conferenceResponse = await PollForConferenceDetails(message); 
                pollCount++;
            } while (conferenceResponse == null);
            
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

        private async Task<ConferenceDetailsResponse> PollForConferenceDetails(HearingConfirmationForParticipantDto message)
        {
            try
            {
                return await _videoApiService.GetConferenceByHearingRefId(message.HearingId, true);
            }
            catch (VideoApiException e)
            {
                if(pollCount >= 3) 
                    throw;
                
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return null;
                }

                throw;
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((NewParticipantHearingConfirmationEvent)integrationEvent);
        }
    }
}