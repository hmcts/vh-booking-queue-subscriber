using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantRemovedHandler : MessageHandlerBase
    {
        public ParticipantRemovedHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.ParticipantRemoved;
        
        public override Task HandleAsync(BookingsMessage bookingsMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}