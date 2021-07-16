using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantsAddedHandler : IMessageHandler<ParticipantsAddedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;

        public ParticipantsAddedHandler(IVideoApiService videoApiService, IVideoWebService videoWebService)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(ParticipantsAddedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            var request = new AddParticipantsToConferenceRequest
            {
                Participants = eventMessage.Participants
                    .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest).ToList()
            };

            await _videoApiService.AddParticipantsToConference(conference.Id, request);


            var updateConferenceParticipantsRequest = new UpdateConferenceParticipantsRequest
            {
                NewParticipants =
                    eventMessage.Participants.Select(x => ParticipantToParticipantRequestMapper.MapToParticipantRequest(x)).ToList(),
            };
            await _videoWebService.PushParticipantsUpdatedMessage(conference.Id, updateConferenceParticipantsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantsAddedIntegrationEvent)integrationEvent);
        }
    }
}