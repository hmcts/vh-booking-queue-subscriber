using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantAddedHandler : IMessageHandler<ParticipantAddedIntegrationEvent>
    {
        public ParticipantAddedHandler(IVideoApiService videoApiService)
        {
        }

        public Task HandleAsync(ParticipantAddedIntegrationEvent eventMessage)
        {
            throw new System.NotImplementedException();
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((ParticipantAddedIntegrationEvent)integrationEvent);
        }
    }
}