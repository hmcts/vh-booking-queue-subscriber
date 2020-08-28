using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public string Sip { get; set; }
        public string DisplayName { get; set; }
    }
}