namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class NewParticipantMultidayHearingConfirmationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
        public int TotalDays { get; set; }
    }
}
