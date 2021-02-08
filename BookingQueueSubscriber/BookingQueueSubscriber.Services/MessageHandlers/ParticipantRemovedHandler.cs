using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantRemovedHandler : IMessageHandler<ParticipantRemovedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly ILogger<ParticipantRemovedHandler> _logger;

        public ParticipantRemovedHandler(IVideoApiService videoApiService, ILogger<ParticipantRemovedHandler> logger)
        {
            _videoApiService = videoApiService;
            _logger = logger;
        }

        public async Task HandleAsync(ParticipantRemovedIntegrationEvent eventMessage)
        {
            _logger.LogDebug(
                "Attempting to remove participant by ref ID {Participant} from conference by hearing id {Hearing}",
                eventMessage.ParticipantId, eventMessage.HearingId);
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var participantResponse =
                conference.Participants.SingleOrDefault(x => x.ParticipantRefIid == eventMessage.ParticipantId);
            if (participantResponse != null)
            {
                _logger.LogError(
                    "Participant with ref ID {Participant} does not exist in conference with  hearing ref id {Hearing}",
                    eventMessage.ParticipantId, eventMessage.HearingId);
                await _videoApiService.RemoveParticipantFromConference(conference.Id, participantResponse.Id);
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantRemovedIntegrationEvent)integrationEvent);
        }
    }
}