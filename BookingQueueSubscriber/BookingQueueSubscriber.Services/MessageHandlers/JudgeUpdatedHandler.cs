using BookingQueueSubscriber.Common.Logging;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using NotificationApi.Client;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class JudgeUpdatedHandler(
        IVideoApiService apiService,
        INotificationApiClient notificationApiClient,
        ILogger logger)
        : IMessageHandler<JudgeUpdatedIntegrationEvent>
    {
        public async Task HandleAsync(JudgeUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await apiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
            var judgeResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Judge.ParticipantId);
            
            if (judgeResponse != null)
            {
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Judge);
                if(judgeResponse.ContactEmail != request.ContactEmail && eventMessage.SendNotification)
                    await SendNotification(eventMessage);
                await apiService.UpdateParticipantDetails(conferenceResponse.Id, judgeResponse.Id, request);
            }
            else
                logger.JudgeParticipantNotFound(eventMessage.Judge.ParticipantId, conferenceResponse.Id);
        }

        private async Task SendNotification(JudgeUpdatedIntegrationEvent eventMessage)
        {
            var request = NotificationRequestHelper.BuildExistingUserSingleDayHearingConfirmationRequest(eventMessage.Hearing, eventMessage.Judge);
            await notificationApiClient.SendParticipantSingleDayHearingConfirmationForExistingUserEmailAsync(request);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent) => await HandleAsync((JudgeUpdatedIntegrationEvent)integrationEvent);
        
    }
}