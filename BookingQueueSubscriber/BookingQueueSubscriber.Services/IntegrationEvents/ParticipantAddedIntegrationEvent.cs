using System;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantAddedIntegrationEvent: IntegrationEvent
    {

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
        public override IntegrationEventType EventType => IntegrationEventType.ParticipantAdded;
    }
}