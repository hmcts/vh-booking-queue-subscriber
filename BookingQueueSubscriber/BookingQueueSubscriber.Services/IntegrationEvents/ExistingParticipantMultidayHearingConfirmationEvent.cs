namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ExistingParticipantMultidayHearingConfirmationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
        public int TotalDays { get; set; }
    }
}
