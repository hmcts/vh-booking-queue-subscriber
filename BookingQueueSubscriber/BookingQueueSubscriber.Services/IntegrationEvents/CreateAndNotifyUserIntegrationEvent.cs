namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class CreateAndNotifyUserIntegrationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
    }
}

    