using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingParticipantsUpdatedHandler : IMessageHandler<HearingParticipantsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId, true);

            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants =
                    eventMessage.ExistingParticipants.Select(x => ParticipantToUpdateParticipantMapper.MapToParticipantRequest(x)).ToList(),
                NewParticipants =
                    eventMessage.NewParticipants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
                RemovedParticipants = eventMessage.RemovedParticipants,
                LinkedParticipants =
                    eventMessage.LinkedParticipants.Select(x => LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest(x)).ToList(),
            };

            await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}