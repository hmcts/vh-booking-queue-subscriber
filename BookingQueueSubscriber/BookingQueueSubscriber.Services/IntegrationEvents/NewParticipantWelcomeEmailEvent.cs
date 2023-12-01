namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class NewParticipantWelcomeEmailEvent: IIntegrationEvent
    {
        public WelcomeEmailDto WelcomeEmail { get; set; }
    }
}
