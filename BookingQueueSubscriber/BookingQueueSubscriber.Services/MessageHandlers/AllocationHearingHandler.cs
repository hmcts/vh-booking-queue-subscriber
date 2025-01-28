using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Client;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class AllocationHearingHandler : IMessageHandler<HearingsAllocatedIntegrationEvent>
    {
        private readonly IVideoWebService _videoWebService;
        private readonly IVideoApiClient _videoApiClient;

        public AllocationHearingHandler(IVideoWebService videoWebService, IVideoApiClient videoApiClient)
        {
            _videoWebService = videoWebService;
            _videoApiClient = videoApiClient;
        }

        public async Task HandleAsync(HearingsAllocatedIntegrationEvent eventMessage)
        {
            DateTime todayDate = DateTime.Now;

            var todayHearings = eventMessage.Hearings.Where(h => h.ScheduledDateTime.Date == todayDate.Date);
            var conferences = await _videoApiClient.GetConferencesByHearingRefIdsAsync(new GetConferencesByHearingIdsRequest()
            {
                IncludeClosed = false,
                HearingRefIds = todayHearings.Select(h => h.HearingId).ToArray()
            });
            var updateAllocationHearingsRequest = new HearingAllocationNotificationRequest
            {
                AllocatedCsoUserName = eventMessage.AllocatedCso.Username,
                ConferenceIds = conferences.Select(c => c.Id).ToList()
            };
            await _videoWebService.PushAllocationToCsoUpdatedMessage(updateAllocationHearingsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingsAllocatedIntegrationEvent) integrationEvent);
        }
    }
}