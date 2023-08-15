namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingNotificationIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}

    