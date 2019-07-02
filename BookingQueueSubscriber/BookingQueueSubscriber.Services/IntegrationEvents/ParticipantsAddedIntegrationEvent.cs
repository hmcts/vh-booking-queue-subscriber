using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantsAddedIntegrationEvent: IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}