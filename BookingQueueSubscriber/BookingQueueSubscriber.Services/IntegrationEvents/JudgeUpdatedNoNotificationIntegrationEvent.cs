namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class JudgeUpdatedNoNotificationIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public ParticipantDto Judge { get; set; }
    }
}