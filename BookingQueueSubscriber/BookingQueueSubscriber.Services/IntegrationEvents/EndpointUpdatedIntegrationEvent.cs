using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public Guid EndpointId { get; set; }
        public string DisplayName { get; set; }
    }
}