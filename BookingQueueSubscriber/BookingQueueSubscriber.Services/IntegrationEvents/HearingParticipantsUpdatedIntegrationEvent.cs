using System;
using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingParticipantsUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> ExistingParticipants { get; set; } = new List<ParticipantDto>();
        public IList<ParticipantDto> NewParticipants { get; set; } = new List<ParticipantDto>();
        public IList<Guid> RemovedParticipants { get; set; } = new List<Guid>();
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; } = new List<LinkedParticipantDto>();
    }
}