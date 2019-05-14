using System;
using BookingQueueSubscriber.Services.IntegrationEvents;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class BookingsMessage
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public IntegrationEvent IntegrationEvent { get; set; }
    }
}