using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingDetailsUpdatedIntegrationEvent : IntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public override IntegrationEventType EventType => IntegrationEventType.HearingDetailsUpdated;
    }
}