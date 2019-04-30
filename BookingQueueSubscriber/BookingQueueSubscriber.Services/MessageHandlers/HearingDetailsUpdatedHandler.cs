using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDetailsUpdatedHandler : MessageHandlerBase
    {
        public HearingDetailsUpdatedHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.HearingDetailsUpdated;
        
        public override Task HandleAsync(IBookingsMessage bookingsMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}