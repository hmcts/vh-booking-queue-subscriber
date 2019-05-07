using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;

namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandler
    {
        IntegrationEventType IntegrationEventType { get; }
        Type BodyType { get; }
        Task HandleAsync(IntegrationEvent integrationEvent);
    }
    
    public abstract class MessageHandlerBase : IMessageHandler
    {
        protected IVideoApiService VideoApiService { get; }
        public abstract IntegrationEventType IntegrationEventType { get; }
        public abstract Type BodyType { get; }

        public abstract Task HandleAsync(IntegrationEvent integrationEvent);

        protected MessageHandlerBase(IVideoApiService videoApiService)
        {
            VideoApiService = videoApiService;
        }
    }
}