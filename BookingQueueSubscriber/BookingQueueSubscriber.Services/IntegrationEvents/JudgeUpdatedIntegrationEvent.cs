namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class JudgeUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public ParticipantDto Judge { get; set; }

        public bool SendNotification { get; set; }
    }
}