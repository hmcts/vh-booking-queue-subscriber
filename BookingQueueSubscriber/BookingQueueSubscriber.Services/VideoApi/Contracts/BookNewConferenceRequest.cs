using System;
using System.Collections.Generic;

namespace BookingQueueSubscriber.Services.VideoApi.Contracts
{
    public class BookNewConferenceRequest
    {
        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
    }
}