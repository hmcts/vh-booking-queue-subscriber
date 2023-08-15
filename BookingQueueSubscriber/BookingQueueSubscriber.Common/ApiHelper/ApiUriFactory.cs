namespace BookingQueueSubscriber.Common.ApiHelper
{
    public class ApiUriFactory
    {
        public ParticipantsEndpoints ParticipantsEndpoints { get; }
        public ConferenceEndpoints ConferenceEndpoints { get; }
        public EndpointForJvsEndpoints EndpointForJvsEndpoints { get; }

        public ApiUriFactory()
        {
            ParticipantsEndpoints = new ParticipantsEndpoints();
            ConferenceEndpoints = new ConferenceEndpoints();
            EndpointForJvsEndpoints = new EndpointForJvsEndpoints();
        }
    }

    public class ParticipantsEndpoints
    {
        private string ApiRoot => "conferences";

        public string AddParticipantsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";

        public string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";
        public string UpdateParticipantDetails(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";
    }

    public class ConferenceEndpoints
    {
        private string ApiRoot => "conferences";
        public string BookNewConference => $"{ApiRoot}";
        public string UpdateConference => $"{ApiRoot}";
        public string DeleteConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
        public string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId}";
    }

    public class EndpointForJvsEndpoints
    {
        private string ApiRoot => "conferences";
        public string AddEndpoint(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/endpoints";
        public string RemoveEndpoint(Guid conferenceId, string sip) => $"{ApiRoot}/{conferenceId}/endpoints/{sip}";
        public string UpdateEndpoint(Guid conferenceId, string sip) => $"{ApiRoot}/{conferenceId}/endpoints/{sip}/displayname";
    }
}