using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public class VideoApiServiceFake : IVideoApiService
    {

        public ConferenceResponse ConferenceResponse { get; set; }
        public int BookNewConferenceCount { get; private set; }
        public int UpdateConferenceCount { get; private set; }
        public int DeleteConferenceCount { get; private set; }
        public int GetConferenceByHearingRefIdCount { get; private set; }
        public int AddParticipantsToConferenceCount { get; private set; }
        public int RemoveParticipantFromConferenceCount { get; private set; }
        public int UpdateParticipantDetailsCount { get; private set; }
        public int AddEndpointToConferenceCount { get; set; }
        public int RemoveEndpointFromConferenceCount { get; set; }
        public int UpdateEndpointInConferenceCount { get; set; }

        public Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            BookNewConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
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

        public Task<ConferenceResponse> GetConferenceByHearingRefId(Guid hearingRefId)
        {
            GetConferenceByHearingRefIdCount++;
            if (ConferenceResponse == null)
            {
                InitConferenceResponse();
            }
            return Task.FromResult(ConferenceResponse);
        }

        public void InitConferenceResponse()
        {
            ConferenceResponse = new ConferenceResponse
            {
                HearingRefId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Participants = new List<ParticipantResponse>
                {
                    new ParticipantResponse {Id = Guid.NewGuid(), RefId = Guid.NewGuid()}
                }
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

        public Task RemoveEndpointFromConference(Guid conferenceId, Guid endpointId)
        {
            RemoveEndpointFromConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateEndpointInConference(Guid conferenceId, Guid endpointId, UpdateEndpointRequest request)
        {
            UpdateEndpointInConferenceCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public void ClearRequests()
        {
            BookNewConferenceCount = UpdateConferenceCount = DeleteConferenceCount = GetConferenceByHearingRefIdCount =
                AddParticipantsToConferenceCount =
                    RemoveParticipantFromConferenceCount = UpdateParticipantDetailsCount = 0;
        }
    }
}