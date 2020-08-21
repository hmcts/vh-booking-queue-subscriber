using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EndpointUpdateHandler : IMessageHandler<EndpointUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public EndpointUpdateHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(EndpointUpdatedIntegrationEvent eventMessage)
        {
            var conference = await _videoApiService.GetConferenceByHearingRefId(eventMessage.HearingId);
            
            await _videoApiService.UpdateEndpointInConference(conference.Id, eventMessage.EndpointId, new UpdateEndpointRequest
            {
                DisplayName = eventMessage.DisplayName
            });
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((EndpointUpdatedIntegrationEvent)integrationEvent);
        }
    }
}