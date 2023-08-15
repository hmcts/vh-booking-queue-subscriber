namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class EndpointAddedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public EndpointDto Endpoint { get; set; }
    }
}