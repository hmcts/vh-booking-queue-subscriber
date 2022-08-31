using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class JudgeUpdatedHandler : IMessageHandler<JudgeUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _apiService;
        private readonly IUserCreationAndNotification _userCreationAndNotification;
        private readonly ILogger _logger;

        public JudgeUpdatedHandler(IVideoApiService apiService, IUserCreationAndNotification userCreationAndNotification, ILogger logger)
        {
            _logger = logger;
            _apiService = apiService;
            _userCreationAndNotification = userCreationAndNotification;
        }

        public async Task HandleAsync(JudgeUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _apiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            var judgeResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Judge.ParticipantId);
            
            if (judgeResponse != null)
            {
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Judge);
                if(judgeResponse.ContactEmail != request.ContactEmail)
                    await _userCreationAndNotification.SendHearingNotificationAsync(eventMessage.Hearing, new[] {eventMessage.Judge});
                await _apiService.UpdateParticipantDetails(conferenceResponse.Id, judgeResponse.Id, request);
            }
            else
                _logger.LogError("Unable to find judge participant by ref id {ParticipantRefId} in {ConferenceId}", eventMessage.Judge.ParticipantId, conferenceResponse.Id);
        } 
        

        async Task IMessageHandler.HandleAsync(object integrationEvent) => await HandleAsync((JudgeUpdatedIntegrationEvent)integrationEvent);
        
    }
}