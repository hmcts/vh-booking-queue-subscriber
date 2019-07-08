using System;
using System.Collections.Generic;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public class ConferenceResponse
    {
        public Guid Id { get; set; }
        public Guid HearingRefId { get; set; }
        public List<ParticipantResponse> Participants { get; set; }
    }
}