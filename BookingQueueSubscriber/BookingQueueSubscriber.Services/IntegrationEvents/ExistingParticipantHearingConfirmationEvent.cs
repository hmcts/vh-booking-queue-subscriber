namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ExistingParticipantHearingConfirmationEvent: IIntegrationEvent
    {
        public ExistingParticipantHearingConfirmationEvent(HearingConfirmationForParticipantDto dto)
        {
            HearingConfirmationForParticipant = dto;
        }
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get;  }
    }
}
