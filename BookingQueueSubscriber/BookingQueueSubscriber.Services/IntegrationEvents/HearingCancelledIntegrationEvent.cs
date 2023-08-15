namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingCancelledIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
    }
}