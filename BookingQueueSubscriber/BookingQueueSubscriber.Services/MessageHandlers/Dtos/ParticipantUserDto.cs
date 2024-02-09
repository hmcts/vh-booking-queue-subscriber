namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class ParticipantUserDto
    {
        public Guid HearingId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string UserRole { get; set; }
    }
}
