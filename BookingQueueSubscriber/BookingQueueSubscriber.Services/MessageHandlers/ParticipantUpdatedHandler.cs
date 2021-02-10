using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantUpdatedHandler : IMessageHandler<ParticipantUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly ILogger<ParticipantUpdatedHandler> _logger;

        public ParticipantUpdatedHandler(IVideoApiService videoApiService, ILogger<ParticipantUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _logger = logger;
        }

        public async Task HandleAsync(ParticipantUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId, true);
            var participantResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Participant.ParticipantId);
            
            if (participantResponse != null)
            {
                _logger.LogError("Unable to find participant by ref id {ParticipantRefId} in {ConferenceId}", eventMessage.Participant.ParticipantId, conferenceResponse.Id);
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Participant);
                await _videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participantResponse.Id, request);
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantUpdatedIntegrationEvent)integrationEvent);
        }
    }
}