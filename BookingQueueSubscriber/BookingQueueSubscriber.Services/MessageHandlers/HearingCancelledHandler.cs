using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingCancelledHandler : MessageHandlerBase
    {
        public HearingCancelledHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override IntegrationEventType IntegrationEventType => IntegrationEventType.HearingCancelled;
        public override Type BodyType { get; }

        public override Task HandleAsync(IntegrationEvent integrationEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}