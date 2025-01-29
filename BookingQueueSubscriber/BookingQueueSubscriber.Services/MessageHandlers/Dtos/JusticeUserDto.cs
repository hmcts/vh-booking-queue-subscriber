namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class JusticeUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string UserRole { get; set; }
    }
}