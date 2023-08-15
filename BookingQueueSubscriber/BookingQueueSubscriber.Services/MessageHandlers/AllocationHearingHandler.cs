using BookingQueueSubscriber.Services.VideoWeb;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class AllocationHearingHandler : IMessageHandler<AllocationHearingsIntegrationEvent>
    {
        private readonly IVideoWebService _videoWebService;

        public AllocationHearingHandler(IVideoWebService videoWebService)
        {
            _videoWebService = videoWebService;
        }

        public async Task HandleAsync(AllocationHearingsIntegrationEvent eventMessage)
        {
            DateTime todayDate = DateTime.Now;

            var todayHearings = eventMessage.Hearings.Where(h => h.ScheduledDateTime.Date == todayDate.Date);

            var updateAllocationHearingsRequest = new AllocationHearingsToCsoRequest
            {
                AllocatedCsoUserName = eventMessage.AllocatedCso.Username,
                Hearings = todayHearings.Select(h =>
                {
                    return new HearingDetailRequest()
                    {
                        Judge = h.JudgeDisplayName, 
                        CaseName = h.CaseName, 
                        Time = h.ScheduledDateTime
                    };
                }).ToList()
            };

            await _videoWebService.PushAllocationToCsoUpdatedMessage(updateAllocationHearingsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((AllocationHearingsIntegrationEvent) integrationEvent);
        }
    }
}