using BookingQueueSubscriber.Services.VideoApi;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantRemovedHandler : IMessageHandler<ParticipantRemovedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public ParticipantRemovedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(ParticipantRemovedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var participantResponse = conference.Participants.SingleOrDefault(x => x.RefId == eventMessage.ParticipantId);
            if (participantResponse != null)
            {
                await _videoApiService.RemoveParticipantFromConference(conference.Id, participantResponse.Id);
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantRemovedIntegrationEvent)integrationEvent);
        }
    }
}