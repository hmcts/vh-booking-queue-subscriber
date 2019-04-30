using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : MessageHandlerBase
    {
        public HearingReadyForVideoHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.HearingIsReadyForVideo;
        
        public override Task HandleAsync(IBookingsMessage bookingsMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}