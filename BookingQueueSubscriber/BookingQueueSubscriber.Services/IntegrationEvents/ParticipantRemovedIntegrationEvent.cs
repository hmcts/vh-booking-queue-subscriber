﻿namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantRemovedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public Guid ParticipantId { get; set; }
    }
}