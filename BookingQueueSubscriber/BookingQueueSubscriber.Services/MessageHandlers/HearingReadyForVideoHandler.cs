using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : MessageHandlerBase
    {
        public HearingReadyForVideoHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.HearingIsReadyForVideo;

        public override async Task HandleAsync(BookingsMessage bookingsMessage)
        {
            var request = (BookNewConferenceRequest) bookingsMessage.Message;
            await VideoApiService.BookNewConferenceAsync(request);
        }
    }
}