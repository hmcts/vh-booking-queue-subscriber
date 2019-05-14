using BookingQueueSubscriber.Services.MessageHandlers;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class IntegrationEvent
    {
        public virtual IntegrationEventType EventType { get; }
    }
}