namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class NewParticipantWelcomeEmailEvent: IIntegrationEvent
    {
        public NewParticipantWelcomeEmailEvent(WelcomeEmailDto dto)
        {
            WelcomeEmail = dto;
        }
        public WelcomeEmailDto WelcomeEmail { get; set; }
    }
}
