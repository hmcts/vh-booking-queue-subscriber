using System.Threading.Tasks;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

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
            var hearing = (HearingDto) bookingsMessage.Message;
            var request = new HearingToBookConferenceMapper().MapToBookNewConferenceRequest(hearing);
            await VideoApiService.BookNewConferenceAsync(request).ConfigureAwait(true);
        }
    }
}