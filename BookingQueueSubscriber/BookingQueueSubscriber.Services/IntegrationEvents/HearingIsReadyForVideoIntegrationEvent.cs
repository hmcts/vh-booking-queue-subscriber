namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingIsReadyForVideoIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
        public IList<EndpointDto> Endpoints { get; set; }
    }
}