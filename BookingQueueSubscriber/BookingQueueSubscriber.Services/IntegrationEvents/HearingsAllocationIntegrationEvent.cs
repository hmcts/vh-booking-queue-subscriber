using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingsAllocationIntegrationEvent : IIntegrationEvent
    {
        public List<HearingDto> Hearings { get; set; }
        public JusticeUserDto AllocatedCso { get; set; }
    }
}