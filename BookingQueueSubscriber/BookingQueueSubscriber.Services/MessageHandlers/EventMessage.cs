using System;
using BookingQueueSubscriber.Services.IntegrationEvents;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class EventMessage
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public IIntegrationEvent IntegrationEvent { get; set; }
    }
}