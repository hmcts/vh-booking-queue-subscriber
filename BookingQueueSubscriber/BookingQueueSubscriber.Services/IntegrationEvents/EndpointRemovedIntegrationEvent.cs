using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointRemovedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public string Sip { get; set; }
    }
}