using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using Microsoft.Extensions.Logging;
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

            var updateAlocatioHearingsRequest = new AllocationHearingsToCsoRequest
            {
                AllocatedCsoUserName = eventMessage.AllocatedCso.Username,
                Hearings = todayHearings.Select(h =>
                {
                    return new HearingDetailRequest()
                    {
                        Judge = h.JudgeDisplayName, 
                        CaseName = h.CaseName, 
                        Time = h.ScheduledDateTime.ToString("HH:mm tt")
                    };
                }).ToList()
            };

            _videoWebService.PushAllocationToCsoUpdatedMessage(updateAlocatioHearingsRequest);
        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((AllocationHearingsIntegrationEvent) integrationEvent);
        }
    }
}