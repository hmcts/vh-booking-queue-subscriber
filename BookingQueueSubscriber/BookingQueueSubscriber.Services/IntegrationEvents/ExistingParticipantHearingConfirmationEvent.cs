namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ExistingParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
    }
}
