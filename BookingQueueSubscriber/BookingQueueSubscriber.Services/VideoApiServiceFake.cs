using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services
{
    public class VideoApiServiceFake : IVideoApiService
    {
        public Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task DeleteConferenceAsync(Guid conferenceId)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task<ConferenceResponse> GetConferenceByHearingRefId(Guid hearingRefId)
        {
            return Task.FromResult(new ConferenceResponse
            {
                HearingRefId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Participants = new List<ParticipantResponse>
                {
                    new ParticipantResponse {Id = Guid.NewGuid(), RefId = Guid.NewGuid()}
                }
            });
        }

        public Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task UpdateParticipantDetails(Guid conferenceId, Guid participantId, UpdateParticipantRequest request)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}