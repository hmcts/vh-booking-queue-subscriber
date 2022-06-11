using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class MultiDayHearingIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; }
        public IList<ParticipantDto> Participants { get; }
        public int TotalDays { get; set; }
    }
}