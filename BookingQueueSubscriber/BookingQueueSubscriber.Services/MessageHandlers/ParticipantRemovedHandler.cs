using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantRemovedHandler : IMessageHandler<ParticipantRemovedIntegrationEvent>
    {
        public ParticipantRemovedHandler(IVideoApiService videoApiService)
        {
        }

        public Task HandleAsync(ParticipantRemovedIntegrationEvent eventMessage)
        {
            throw new System.NotImplementedException();
        }
        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantRemovedIntegrationEvent)integrationEvent);
        }
    }
}