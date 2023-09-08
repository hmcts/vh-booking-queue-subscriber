using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingParticipantsUpdatedHandler : IMessageHandler<HearingParticipantsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly ILogger<HearingParticipantsUpdatedHandler> _logger;


        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, 
            IVideoWebService videoWebService, 
            IUserCreationAndNotification userCreationAndNotification, 
            ILogger<HearingParticipantsUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
            _logger = logger;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            try
            {
                _logger.LogInformation("GettingConference be hearing ref Id: {HearingId}", eventMessage.Hearing.HearingId);
                var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
                
                _logger.LogInformation("Successfully got Conference: {@ConferenceResponse}", conferenceResponse);
                var newParticipantUsers = await _userCreationAndNotification.CreateUserAndNotifcationAsync(eventMessage.Hearing, eventMessage.NewParticipants);
                
                _logger.LogInformation("Sending User Notification for: {@Hearing} with {@NewParticipants} ", eventMessage.Hearing, eventMessage.NewParticipants);
                await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, eventMessage.NewParticipants);
                
                _logger.LogInformation("Successfully send notification for: {@NewParticipants}", eventMessage.NewParticipants);

                var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
                {
                    ExistingParticipants = eventMessage.ExistingParticipants.Select(x => ParticipantToUpdateParticipantMapper.MapToParticipantRequest(x)).ToList(),
                    NewParticipants = eventMessage.NewParticipants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
                    RemovedParticipants = eventMessage.RemovedParticipants,
                    LinkedParticipants = eventMessage.LinkedParticipants.Select(x => LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest(x)).ToList(),
                };
                _logger.LogInformation("Created updateParticipantRequest for conference {Id} to send to videoApi {@UpdateConferenceParticipantsRequest}", 
                    conferenceResponse.Id, updateConferenceParticipantsRequest);
                await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
                
                _logger.LogInformation("Pushing participant update message to video-web conference:{Id}  request:{@UpdateConferenceParticipantsRequest}", 
                    conferenceResponse.Id, updateConferenceParticipantsRequest);
                await _videoWebService.PushParticipantsUpdatedMessage(conferenceResponse.Id, updateConferenceParticipantsRequest);
                
                _logger.LogInformation("AssigningUserToGroup: {@NewParticipantUsers}", newParticipantUsers);
                await _userCreationAndNotification.HandleAssignUserToGroup(newParticipantUsers);
                
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error handling HearingParticipantsUpdatedIntegrationEvent");
                throw;
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}