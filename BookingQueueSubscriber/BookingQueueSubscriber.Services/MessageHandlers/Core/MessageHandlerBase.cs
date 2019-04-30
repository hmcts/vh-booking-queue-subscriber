using System.Threading.Tasks;

namespace BookingQueueSubscriber.Services.MessageHandlers.Core
{
    public interface IMessageHandler
    {
        MessageType MessageType { get; }
        Task HandleAsync(BookingsMessage bookingsMessage);
    }
    
    public abstract class MessageHandlerBase : IMessageHandler
    {
        protected IVideoApiService VideoApiService { get; }
        public abstract MessageType MessageType { get; }
        public abstract Task HandleAsync(BookingsMessage bookingsMessage);

        protected MessageHandlerBase(IVideoApiService videoApiService)
        {
            VideoApiService = videoApiService;
        }
    }
}