using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingDateTimeChangedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; }
        public DateTime OldScheduledDateTime { get; }
        public IList<ParticipantDto> Participants { get; }
    }
}