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
        private readonly ILogger<HearingParticipantsUpdatedHandler> _logger;


        public HearingParticipantsUpdatedHandler(IVideoApiService videoApiService, 
            IVideoWebService videoWebService, 
            ILogger<HearingParticipantsUpdatedHandler> logger)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _logger = logger;
        }

        public async Task HandleAsync(HearingParticipantsUpdatedIntegrationEvent eventMessage)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError(e, "Error handling HearingParticipantsUpdatedIntegrationEvent");
                throw;
            }
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingParticipantsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}