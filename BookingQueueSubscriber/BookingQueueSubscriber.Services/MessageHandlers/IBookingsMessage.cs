namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public interface IBookingsMessage
    {
        MessageType EventType { get; }
    }
}