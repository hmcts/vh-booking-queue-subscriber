using System;
using BookingQueueSubscriber.Services.MessageHandlers;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantRemovedIntegrationEvent : IntegrationEvent
    {
        public ParticipantRemovedIntegrationEvent(Guid hearingId, Guid participantId)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
        }

        public Guid HearingId { get; }
        public Guid ParticipantId { get; }

        public override IntegrationEventType EventType => IntegrationEventType.ParticipantRemoved;
    }
}