using System;
using System.Collections.Generic;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class HearingDto
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int ScheduledDuration { get; set; }
        public string CaseType { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}