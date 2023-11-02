namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class NewParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }

    }
}
