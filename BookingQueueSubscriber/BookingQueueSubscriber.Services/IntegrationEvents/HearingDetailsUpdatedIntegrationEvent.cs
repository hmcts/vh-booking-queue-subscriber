namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingDetailsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
    }
}