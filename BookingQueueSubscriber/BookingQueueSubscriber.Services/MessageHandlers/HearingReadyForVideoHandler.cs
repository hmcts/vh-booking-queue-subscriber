using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : MessageHandlerBase
    {
        public HearingReadyForVideoHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override IntegrationEventType IntegrationEventType => IntegrationEventType.HearingIsReadyForVideo;
        public override Type BodyType => typeof(HearingIsReadyForVideoIntegrationEvent);

        public override async Task HandleAsync(IntegrationEvent integrationEvent)
        {
            var hearingReadyEvent = (HearingIsReadyForVideoIntegrationEvent) integrationEvent;
            if (hearingReadyEvent == null)
            {
                throw new ArgumentNullException(nameof(integrationEvent),
                    $"Expected message to be of type {typeof(HearingIsReadyForVideoIntegrationEvent)}");
            }
            var request =
                new HearingToBookConferenceMapper().MapToBookNewConferenceRequest(hearingReadyEvent.Hearing,
                    hearingReadyEvent.Participants);
            await VideoApiService.BookNewConferenceAsync(request).ConfigureAwait(false);
        }
    }
}