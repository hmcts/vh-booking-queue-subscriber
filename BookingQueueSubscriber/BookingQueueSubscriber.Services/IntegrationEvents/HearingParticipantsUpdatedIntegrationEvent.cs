using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingParticipantsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public Guid HearingId { get; set; }
        public IList<ParticipantDto> ExistingParticipants { get; set; }
        public IList<ParticipantDto> NewParticipants { get; set; }
        public IList<Guid> RemovedParticipants { get; set; }
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }
    }
}