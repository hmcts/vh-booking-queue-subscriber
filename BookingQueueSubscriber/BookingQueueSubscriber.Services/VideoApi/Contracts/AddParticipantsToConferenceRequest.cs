using System.Collections.Generic;

namespace BookingQueueSubscriber.Services.VideoApi.Contracts
{
    public class AddParticipantsToConferenceRequest
    {
        public List<ParticipantRequest> Participants { get; set; }
    }
}