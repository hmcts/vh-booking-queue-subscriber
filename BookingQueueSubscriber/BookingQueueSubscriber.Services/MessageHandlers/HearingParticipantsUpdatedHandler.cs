using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.NotificationApi;
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
        private readonly INotificationService _notificationService;
        private readonly ILogger<HearingParticipantsUpdatedHandler> _logger;


        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, 
            IVideoWebService videoWebService, 
            IUserCreationAndNotification userCreationAndNotification, 
            ILogger<HearingParticipantsUpdatedHandler> logger, INotificationService notificationService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _userCreationAndNotification = userCreationAndNotification;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            try
            {
                var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
                
                var newParticipantUsers = await _userCreationAndNotification.CreateUserAndSendNotificationAsync(eventMessage.Hearing, eventMessage.NewParticipants);
                
                await _notificationService.SendNewSingleDayHearingConfirmationNotification(eventMessage.Hearing, eventMessage.NewParticipants);
                
                var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
                {
                    ExistingParticipants = eventMessage.ExistingParticipants.Select(ParticipantToUpdateParticipantMapper.MapToParticipantRequest).ToList(),
                    NewParticipants = eventMessage.NewParticipants.Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList(),
                    RemovedParticipants = eventMessage.RemovedParticipants,
                    LinkedParticipants = eventMessage.LinkedParticipants.Select(LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest).ToList(),
                };
                await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
                
                await _videoWebService.PushParticipantsUpdatedMessage(conferenceResponse.Id, updateConferenceParticipantsRequest);
                
                await _userCreationAndNotification.AssignUserToGroupForHearing(newParticipantUsers);
                
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