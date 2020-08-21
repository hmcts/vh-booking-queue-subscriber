using System;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointAddedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; }
        public EndpointDto Endpoint { get; }
    }
}