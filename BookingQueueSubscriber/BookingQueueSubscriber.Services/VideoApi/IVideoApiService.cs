using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public interface IVideoApiService
    {
        Task<ConferenceDetailsResponse> BookNewConferenceAsync(BookNewConferenceRequest request);
        Task UpdateConferenceAsync(UpdateConferenceRequest request);
        Task DeleteConferenceAsync(Guid conferenceId);
        Task<ConferenceDetailsResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false);
        Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request);
        Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId);
        Task UpdateConferenceParticipantsAsync(Guid conferenceId, UpdateConferenceParticipantsRequest request);
        Task UpdateParticipantDetails(Guid conferenceId, Guid participantId, UpdateParticipantRequest request);
        Task<ICollection<EndpointResponse>> GetEndpointsForConference(Guid conferenceId);
        Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request);
        Task RemoveEndpointFromConference(Guid conferenceId, string sip);
        Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request);
        Task CloseConsultation(Guid conferenceId, Guid participantId);
        Task UpdateParticipantDetailsWithPolling(Guid hearingId, string username, HearingConfirmationForParticipantDto message);
    }
}