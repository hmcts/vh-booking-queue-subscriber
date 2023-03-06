using System;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class HearingAllocationDto : HearingDto
    {
        public string JudgeDisplayName { get; set;}
    }
}