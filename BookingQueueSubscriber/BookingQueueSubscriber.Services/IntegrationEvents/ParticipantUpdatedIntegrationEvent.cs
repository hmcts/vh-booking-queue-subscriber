﻿namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public ParticipantDto Participant { get; set; }
    }
}