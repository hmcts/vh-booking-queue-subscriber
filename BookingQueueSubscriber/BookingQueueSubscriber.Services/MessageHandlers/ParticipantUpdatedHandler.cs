using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantUpdatedHandler : IMessageHandler<ParticipantUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public ParticipantUpdatedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(ParticipantUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var participantResponse = conferenceResponse.Participants.SingleOrDefault(x => x.RefId == eventMessage.Participant.ParticipantId);
            if (participantResponse != null)
            {
                await _videoApiService.UpdateParticipantDetails(conferenceResponse.Id, participantResponse.Id,
                    new UpdateParticipantRequest
                    {
                        Fullname = eventMessage.Participant.Fullname,
                        DisplayName = eventMessage.Participant.DisplayName,
                        Representee = eventMessage.Participant.Representee
                    });
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantUpdatedIntegrationEvent)integrationEvent);
        }
    }
}