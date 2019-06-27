using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDetailsUpdatedHandler : MessageHandlerBase
    {
        public HearingDetailsUpdatedHandler(IVideoApiService videoApiService) : base(videoApiService)
        {
        }

        public override IntegrationEventType IntegrationEventType => IntegrationEventType.HearingDetailsUpdated;
        public override Type BodyType => typeof(HearingDetailsUpdatedIntegrationEvent);

        public override async Task HandleAsync(IntegrationEvent integrationEvent)
        {
            if (!(integrationEvent is HearingDetailsUpdatedIntegrationEvent updatedIntegrationEvent))
            {
                throw new ArgumentNullException(nameof(integrationEvent),
                    $"Expected message to be of type {typeof(HearingIsReadyForVideoIntegrationEvent)}");
            }

            var hearing = updatedIntegrationEvent.Hearing;
            var request = new UpdateConferenceRequest
            {
                HearingRefId = hearing.HearingId,
                CaseName = hearing.CaseName,
                CaseNumber = hearing.CaseNumber,
                CaseType = hearing.CaseType,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration
            };
            
            await VideoApiService.UpdateConferenceAsync(request).ConfigureAwait(false);
        }
    }
}