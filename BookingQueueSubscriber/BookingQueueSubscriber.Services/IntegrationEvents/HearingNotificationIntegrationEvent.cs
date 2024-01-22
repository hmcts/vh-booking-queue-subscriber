namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingNotificationIntegrationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
    }
}

    