namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ExistingParticipantMultidayHearingConfirmationEvent : IIntegrationEvent
    {
        public ExistingParticipantMultidayHearingConfirmationEvent(HearingConfirmationForParticipantDto dto, int totalDays)
        {
            HearingConfirmationForParticipant = dto;
            TotalDays = totalDays;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }
        public int TotalDays { get; }
    }
}
