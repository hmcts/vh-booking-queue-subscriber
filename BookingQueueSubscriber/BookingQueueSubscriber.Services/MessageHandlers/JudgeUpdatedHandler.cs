using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using NotificationApi.Client;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class JudgeUpdatedHandler : IMessageHandler<JudgeUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _apiService;
        private readonly INotificationApiClient _notificationApiClient;
        private readonly ILogger _logger;

        public JudgeUpdatedHandler(IVideoApiService apiService, INotificationApiClient notificationApiClient, ILogger logger)
        {
            _logger = logger;
            _apiService = apiService;
            _notificationApiClient = notificationApiClient;
        }

        public async Task HandleAsync(JudgeUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _apiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            var judgeResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Judge.ParticipantId);
            
            if (judgeResponse != null)
            {
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Judge);
                if(judgeResponse.ContactEmail != request.ContactEmail && eventMessage.SendNotification)
                    await SendNotification(eventMessage);
                await _apiService.UpdateParticipantDetails(conferenceResponse.Id, judgeResponse.Id, request);
            }
            else
                _logger.LogError("Unable to find judge participant by ref id {ParticipantRefId} in {ConferenceId}", eventMessage.Judge.ParticipantId, conferenceResponse.Id);
        }

        private async Task SendNotification(JudgeUpdatedIntegrationEvent eventMessage)
        {
            var request = NotificationRequestHelper.BuildExistingUserSingleDayHearingConfirmationRequest(eventMessage.Hearing, eventMessage.Judge);
            await _notificationApiClient.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent) => await HandleAsync((JudgeUpdatedIntegrationEvent)integrationEvent);
        
    }
}