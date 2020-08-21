using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; }
        public Guid EndpointId { get; }
        public string DisplayName { get; }
    }
}