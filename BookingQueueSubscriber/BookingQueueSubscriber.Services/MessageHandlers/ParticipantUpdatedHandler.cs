using BookingQueueSubscriber.Common.Logging;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers;

public class ParticipantUpdatedHandler(
    IVideoApiService videoApiService,
    ILogger<ParticipantUpdatedHandler> logger,
    IUserService userService)
    : IMessageHandler<ParticipantUpdatedIntegrationEvent>
{
    public async Task HandleAsync(ParticipantUpdatedIntegrationEvent eventMessage)
    {
        var conferenceResponse = await videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId, true);
        logger.UpdatingParticipantList(conferenceResponse.Id);
        var participantResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Participant.ParticipantId);
            
        if (participantResponse != null)
        {
            logger.ParticipantNotFound(eventMessage.Participant.ParticipantId, conferenceResponse.Id);
            var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Participant);
            await videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participantResponse.Id, request);

            var existingContactEmail = participantResponse.ContactEmail;
            var newContactEmail = eventMessage.Participant.ContactEmail;
            if (newContactEmail.Trim() != existingContactEmail.Trim())
            {
                await userService.UpdateUserContactEmail(existingContactEmail, newContactEmail);
            }
        }
    }

    async Task IMessageHandler.HandleAsync(object integrationEvent)
    {
        await HandleAsync((ParticipantUpdatedIntegrationEvent)integrationEvent);
    }
}