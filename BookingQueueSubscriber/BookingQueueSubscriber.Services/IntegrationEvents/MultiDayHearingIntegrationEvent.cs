namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class MultiDayHearingIntegrationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
        public int TotalDays { get; set; }
    }
}