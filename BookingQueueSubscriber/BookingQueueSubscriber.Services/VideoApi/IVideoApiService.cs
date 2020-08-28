using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public interface IVideoApiService
    {
        Task BookNewConferenceAsync(BookNewConferenceRequest request);
        Task UpdateConferenceAsync(UpdateConferenceRequest request);
        Task DeleteConferenceAsync(Guid conferenceId);
        Task<ConferenceResponse> GetConferenceByHearingRefId(Guid hearingRefId);
        Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request);
        Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId);
        Task UpdateParticipantDetails(Guid conferenceId, Guid participantId, UpdateParticipantRequest request);
        Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request);
        Task RemoveEndpointFromConference(Guid conferenceId, string sip);
        Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request);
    }
}