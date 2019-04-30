namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class BookingsMessage
    {
        public MessageType EventType { get; set; }
        public object Message { get; set; }
    }
}