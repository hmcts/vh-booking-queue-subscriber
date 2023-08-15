namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingDateTimeChangedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public DateTime OldScheduledDateTime { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}