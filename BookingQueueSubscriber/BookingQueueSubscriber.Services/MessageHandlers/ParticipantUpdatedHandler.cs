using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantUpdatedHandler : IMessageHandler<ParticipantUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly ILogger<ParticipantUpdatedHandler> _logger;
        private readonly IUserService _userService;

        public ParticipantUpdatedHandler(IVideoApiService videoApiService, ILogger<ParticipantUpdatedHandler> logger, IUserService userService)
        {
            _videoApiService = videoApiService;
            _logger = logger;
            _userService = userService;
        }

        public async Task HandleAsync(ParticipantUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId, true);
            _logger.LogInformation("Update participant list for Conference {ConferenceId}", conferenceResponse.Id);
            var participantResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Participant.ParticipantId);
            
            if (participantResponse != null)
            {
                _logger.LogError("Unable to find participant by ref id {ParticipantRefId} in {ConferenceId}", eventMessage.Participant.ParticipantId, conferenceResponse.Id);
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Participant);
                await _videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participantResponse.Id, request);

                var existingContactEmail = participantResponse.ContactEmail;
                var newContactEmail = eventMessage.Participant.ContactEmail;
                if (newContactEmail.Trim() != existingContactEmail.Trim())
                {
                    await _userService.UpdateUserContactEmail(existingContactEmail, newContactEmail);
                }
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantUpdatedIntegrationEvent)integrationEvent);
        }
    }
}