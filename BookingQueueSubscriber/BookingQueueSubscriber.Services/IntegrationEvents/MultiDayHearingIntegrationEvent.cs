namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class MultiDayHearingIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
        public int TotalDays { get; set; }
    }
}