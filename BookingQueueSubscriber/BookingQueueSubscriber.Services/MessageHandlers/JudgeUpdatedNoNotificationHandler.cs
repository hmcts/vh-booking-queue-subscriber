using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class JudgeUpdatedNoNotificationHandler : IMessageHandler<JudgeUpdatedNoNotificationIntegrationEvent>
    {
        private readonly IVideoApiService _apiService;
        private readonly ILogger _logger;

        public JudgeUpdatedNoNotificationHandler(IVideoApiService apiService, ILogger logger)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task HandleAsync(JudgeUpdatedNoNotificationIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _apiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            var judgeResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Judge.ParticipantId);
            
            if (judgeResponse != null)
            {
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Judge);
                await _apiService.UpdateParticipantDetails(conferenceResponse.Id, judgeResponse.Id, request);
            }
            else
                _logger.LogError("Unable to find judge participant by ref id {ParticipantRefId} in {ConferenceId}", eventMessage.Judge.ParticipantId, conferenceResponse.Id);
        } 
        

        async Task IMessageHandler.HandleAsync(object integrationEvent) => await HandleAsync((JudgeUpdatedNoNotificationIntegrationEvent)integrationEvent);
        
    }
}