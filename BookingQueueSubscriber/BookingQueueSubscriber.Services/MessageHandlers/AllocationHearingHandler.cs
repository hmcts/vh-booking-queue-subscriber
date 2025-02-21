using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class AllocationHearingHandler : IMessageHandler<HearingsAllocatedIntegrationEvent>
    {
        private readonly IVideoWebService _videoWebService;
        private readonly IVideoApiService _videoApiService;

        public AllocationHearingHandler(IVideoWebService videoWebService, IVideoApiService videoApiService)
        {
            _videoWebService = videoWebService;
            _videoApiService = videoApiService;
        }

        public async Task HandleAsync(HearingsAllocatedIntegrationEvent eventMessage)
        {
            DateTime todayDate = DateTime.Now;

            var todayHearings = eventMessage.Hearings.Where(h => h.ScheduledDateTime.Date == todayDate.Date);
            var conferences = await _videoApiService.GetConferencesByHearingRefIdsAsync(new GetConferencesByHearingIdsRequest()
            {
                IncludeClosed = false,
                HearingRefIds = todayHearings.Select(h => h.HearingId).ToArray()
            });
            var updateAllocationHearingsRequest = new HearingAllocationNotificationRequest
            {
                AllocatedCsoUserName = eventMessage.AllocatedCso.Username,
                AllocatedCsoFullName = eventMessage.AllocatedCso.FullName,
                AllocatedCsoUserId = eventMessage.AllocatedCso.UserId,
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