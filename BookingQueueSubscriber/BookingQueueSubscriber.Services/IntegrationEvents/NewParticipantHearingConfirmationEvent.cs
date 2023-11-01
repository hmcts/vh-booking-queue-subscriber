namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class NewParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public NewParticipantHearingConfirmationEvent(HearingConfirmationForParticipantDto dto)
        {
            HearingConfirmationForParticipant = dto;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; }

    }
}
