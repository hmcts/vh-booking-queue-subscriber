using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointUpdatedHandler : IMessageHandler<EndpointUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public EndpointUpdatedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            ParticipantDetailsResponse defenceAdvocate = null;
            if (!string.IsNullOrEmpty(eventMessage.DefenceAdvocate))
            {
                defenceAdvocate = conference.Participants.Single(x => x.ContactEmail ==
                    eventMessage.DefenceAdvocate);
            }
            await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.Sip, new UpdateEndpointRequest
            {
                DisplayName = eventMessage.DisplayName,
                DefenceAdvocate = defenceAdvocate?.Username
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
    }
}