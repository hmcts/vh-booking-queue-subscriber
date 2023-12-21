using System.Diagnostics.CodeAnalysis;
using System.Net;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoApi
{
    [ExcludeFromCodeCoverage]
    public class VideoApiServiceFake : IVideoApiService
    {
        public ConferenceDetailsResponse ConferenceResponse { get; set; }
        public ICollection<EndpointResponse> EndpointResponses { get; set; }
        public int BookNewConferenceCount { get; private set; }
        public int UpdateConferenceCount { get; private set; }
        public int DeleteConferenceCount { get; private set; }
        public int GetConferenceByHearingRefIdCount { get; private set; }
        public int GetEndpointsForConferenceCount { get; private set; }
        public int AddParticipantsToConferenceCount { get; private set; }
        public int RemoveParticipantFromConferenceCount { get; private set; }
        public int UpdateParticipantDetailsCount { get; private set; }
        public int AddEndpointToConferenceCount { get; set; }
        public int RemoveEndpointFromConferenceCount { get; set; }
        public int UpdateEndpointInConferenceCount { get; set; }
        public int UpdateConferenceParticipantsAsyncCount { get; set; }

        public Task<ConferenceDetailsResponse> BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            BookNewConferenceCount++;
            if (ConferenceResponse == null)
            {
                InitConferenceResponse();
            }
            return Task.FromResult(ConferenceResponse);
        }

        public Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            UpdateConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task DeleteConferenceAsync(Guid conferenceId)
        {
            DeleteConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task<ConferenceDetailsResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false)
        {
            GetConferenceByHearingRefIdCount++;
            if (ConferenceResponse == null)
            {
                InitConferenceResponse();
            }
            return Task.FromResult(ConferenceResponse);
        }

        public Task<ICollection<EndpointResponse>> GetEndpointsForConference(Guid conferenceId)
        {
            GetEndpointsForConferenceCount++;
            if (EndpointResponses == null)
            {
                InitEndpointResponse();
            }
            return Task.FromResult(EndpointResponses);
        }

        public void InitConferenceResponse()
        {
            ConferenceResponse = new ConferenceDetailsResponse
            {
                Id = Guid.NewGuid(),
                Participants = new List<ParticipantDetailsResponse>
                {
                    new ParticipantDetailsResponse {Id = Guid.NewGuid(), ContactEmail = "Automation_1316542910@hmcts.net"}
                }
            };
        }

        public void InitEndpointResponse()
        {
            EndpointResponses = new List<EndpointResponse>
            {
               new EndpointResponse { Id = Guid.NewGuid() }
            };
        }

        public Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            AddParticipantsToConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            RemoveParticipantFromConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateParticipantDetails(Guid conferenceId, Guid participantId, UpdateParticipantRequest request)
        {
            UpdateParticipantDetailsCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
        {
            AddEndpointToConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task RemoveEndpointFromConference(Guid conferenceId, string sip)
        {
            RemoveEndpointFromConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request)
        {
            UpdateEndpointInConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task CloseConsultation(Guid conferenceId, Guid participantId)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    
        public void ClearRequests()
        {
            BookNewConferenceCount = UpdateConferenceCount = DeleteConferenceCount = GetConferenceByHearingRefIdCount =
                AddParticipantsToConferenceCount =
                    RemoveParticipantFromConferenceCount = UpdateParticipantDetailsCount = 0;
        }

        public Task UpdateConferenceParticipantsAsync(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            UpdateConferenceParticipantsAsyncCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateParticipantUsernameWithPolling(Guid hearingId, string username, Guid participantId)
        {
            UpdateParticipantDetailsCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}