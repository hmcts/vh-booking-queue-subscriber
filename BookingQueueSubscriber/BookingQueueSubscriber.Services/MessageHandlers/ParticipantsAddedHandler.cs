using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantsAddedHandler : IMessageHandler<ParticipantsAddedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly ILogger<ParticipantsAddedHandler> _logger;

        public ParticipantsAddedHandler(IVideoApiService videoApiService, ILogger<ParticipantsAddedHandler> logger)
        {
            _videoApiService = videoApiService;
            _logger = logger;
        }

        public async Task HandleAsync(ParticipantsAddedIntegrationEvent eventMessage)
        {
            _logger.LogDebug("Attempting to add participants {ParticipantCount} to conference by hearing id {Hearing}",
                eventMessage.Participants.Count, eventMessage.HearingId);
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            await _videoApiService.AddParticipantsToConference(conference.Id, new AddParticipantsToConferenceRequest
            {
                Participants = eventMessage.Participants
                    .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList()
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantsAddedIntegrationEvent)integrationEvent);
        }
    }
}