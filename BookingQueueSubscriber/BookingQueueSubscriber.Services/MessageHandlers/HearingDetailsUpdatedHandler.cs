using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingDetailsUpdatedHandler : IMessageHandler<HearingDetailsUpdatedIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;

        public HearingDetailsUpdatedHandler(IVideoApiService videoApiService)
        {
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingDetailsUpdatedIntegrationEvent eventMessage)
        {
            var hearing = eventMessage.Hearing;
            var request = new UpdateConferenceRequest
            {
                HearingRefId = hearing.HearingId,
                CaseName = hearing.CaseName,
                CaseNumber = hearing.CaseNumber,
                CaseType = hearing.CaseType,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration
            };

            await _videoApiService.UpdateConferenceAsync(request).ConfigureAwait(false);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingDetailsUpdatedIntegrationEvent)integrationEvent);
        }
    }
}