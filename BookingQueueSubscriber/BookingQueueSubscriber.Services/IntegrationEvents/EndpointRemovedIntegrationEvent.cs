using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointRemovedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; }
        public Guid EndpointId { get; }
    }
}