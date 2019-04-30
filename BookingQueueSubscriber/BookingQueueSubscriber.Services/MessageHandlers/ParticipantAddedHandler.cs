using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class ParticipantAddedHandler : MessageHandlerBase
    {
        public ParticipantAddedHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override MessageType MessageType => MessageType.ParticipantAdded;
        
        public override Task HandleAsync(IBookingsMessage bookingsMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}