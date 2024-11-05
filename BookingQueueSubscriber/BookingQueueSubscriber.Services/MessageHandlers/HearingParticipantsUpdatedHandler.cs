using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingParticipantsUpdatedHandler : IMessageHandler<HearingParticipantsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, 
            IVideoWebService videoWebService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            var conferenceResponse = await _videoApiService.GetConferenceByHearingRefId(eventMessage.Hearing.HearingId, true);
                
            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants = eventMessage.ExistingParticipants.Select(ParticipantToUpdateParticipantMapper.MapToParticipantRequest).ToList(),
                NewParticipants = eventMessage.NewParticipants.Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList(),
                RemovedParticipants = eventMessage.RemovedParticipants,
                LinkedParticipants = eventMessage.LinkedParticipants.Select(LinkedParticipantToRequestMapper.MapToLinkedParticipantRequest).ToList(),
            };
            await _videoApiService.UpdateConferenceParticipantsAsync(conferenceResponse.Id, updateConferenceParticipantsRequest);
       
            await _videoWebService.PushParticipantsUpdatedMessage(conferenceResponse.Id, updateConferenceParticipantsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}