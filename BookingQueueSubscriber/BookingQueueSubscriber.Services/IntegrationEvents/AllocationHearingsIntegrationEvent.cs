using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class AllocationHearingsIntegrationEvent: IIntegrationEvent
    {
        public IList<HearingDto> Hearings { get; set; }

        public UserDto AllocatedCso { get; set; }
    }
}