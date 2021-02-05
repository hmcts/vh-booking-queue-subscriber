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
        private ILogger<EndpointRemovedHandler> _logger;

        public ParticipantUpdatedHandler(IVideoApiService videoApiService, ILogger<EndpointRemovedHandler> logger)
        {
            _videoApiService = videoApiService;
            _logger = logger;
        }

        public async Task HandleAsync(ParticipantUpdatedIntegrationEvent eventMessage)
        {
            _logger.LogDebug("Attempting to get conference by hearing id {Hearing}", eventMessage.HearingId);
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var participantResponse = conferenceResponse.Participants.SingleOrDefault(x => x.ParticipantRefIid == eventMessage.Participant.ParticipantId);
            
            if (participantResponse != null)
            {
                _logger.LogError("Found participant {Participant} in hearing {Hearing}",
                    eventMessage.Participant.ParticipantId, eventMessage.HearingId);
                var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(eventMessage.Participant);
                await _videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participantResponse.Id, request);
            }

            _logger.LogError("Unable to find participant {Participant} in hearing {Hearing}",
                eventMessage.Participant.ParticipantId, eventMessage.HearingId);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantUpdatedIntegrationEvent)integrationEvent);
        }
    }
}