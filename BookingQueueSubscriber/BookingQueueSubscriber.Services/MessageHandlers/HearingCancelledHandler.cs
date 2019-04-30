using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingCancelledHandler : MessageHandlerBase
    {
        public HearingCancelledHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.HearingCancelled;
        
        public override Task HandleAsync(IBookingsMessage bookingsMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}