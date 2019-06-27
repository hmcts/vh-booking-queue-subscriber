using System;

namespace BookingQueueSubscriber.Common.ApiHelper
{
    public class ApiUriFactory
    {
        public ParticipantsEndpoints ParticipantsEndpoints { get; }
        public ConferenceEndpoints ConferenceEndpoints { get; }

        public ApiUriFactory()
        {
            ParticipantsEndpoints = new ParticipantsEndpoints();
            ConferenceEndpoints = new ConferenceEndpoints();
        }
    }

    public class ParticipantsEndpoints
    {
        private string ApiRoot => "conferences";

        public string AddParticipantsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";

        public string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";
    }

    public class ConferenceEndpoints
    {
        private string ApiRoot => "conferences";
        public string BookNewConference => $"{ApiRoot}";
        public string UpdateConference => $"{ApiRoot}";
    }
}