namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class CreateUserIntegrationEvent : IIntegrationEvent
    {
        public ParticipantUserDto Participant { get; set; }
    }
}
