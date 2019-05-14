using System;
using BookingQueueSubscriber.Services.MessageHandlers;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingCancelledIntegrationEvent : IntegrationEvent
    {
        public Guid HearingId { get; set; }
        public override IntegrationEventType EventType => IntegrationEventType.HearingCancelled;
    }
}