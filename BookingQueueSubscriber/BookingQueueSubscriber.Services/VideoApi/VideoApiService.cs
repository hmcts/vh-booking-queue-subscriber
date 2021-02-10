using System;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public class VideoApiService : IVideoApiService
    {
        private readonly IVideoApiClient _apiClient;
        private readonly ILogger<VideoApiService> _log;

        public VideoApiService(IVideoApiClient apiClient, ILoggerFactory factory)
        {
            _apiClient = apiClient;
            _log = factory.CreateLogger<VideoApiService>();
        }

        public Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _log.LogInformation("Booking new conference for hearing {Hearing}", request.HearingRefId);
            return _apiClient.BookNewConferenceAsync(request);
        }

        public Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            _log.LogInformation("Updating conference with hearing id {Hearing}", request.HearingRefId);
            return _apiClient.UpdateConferenceAsync(request);
        }

        public Task DeleteConferenceAsync(Guid conferenceId)
        {
            _log.LogInformation("Deleting conference id {ConferenceId}", conferenceId);
            return _apiClient.RemoveConferenceAsync(conferenceId);
        }

        public Task<ConferenceDetailsResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false)
        {
            _log.LogInformation("Getting conference by hearing ref id {HearingId}", hearingRefId);
            return _apiClient.GetConferenceByHearingRefIdAsync(hearingRefId, includeClosed);
        }

        public Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            _log.LogInformation("Adding participants to conference {ConferenceId}", conferenceId);
            return _apiClient.AddParticipantsToConferenceAsync(conferenceId, request);
        }

        public Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            _log.LogInformation("Removing participant {ParticipantId} from conference {ConferenceId}", participantId,
                conferenceId);
            return _apiClient.RemoveParticipantFromConferenceAsync(conferenceId, participantId);
        }

        public Task UpdateParticipantDetails(Guid conferenceId, Guid participantId,
            UpdateParticipantRequest request)
        {
            _log.LogInformation("Updating participant {ParticipantId} in conference {ConferenceId}", participantId,
                conferenceId);
            return _apiClient.UpdateParticipantDetailsAsync(conferenceId, participantId, request);
        }

        public Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
        {
            _log.LogInformation("Adding endpoint to conference: {ConferenceId}", conferenceId);
            return _apiClient.AddEndpointToConferenceAsync(conferenceId, request);
        }

        public Task RemoveEndpointFromConference(Guid conferenceId, string sip)
        {
            _log.LogInformation("Removing endpoint {Sip} from conference {ConferenceId}", sip, conferenceId);
            return _apiClient.RemoveEndpointFromConferenceAsync(conferenceId, sip);
        }

        public Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request)
        {
            _log.LogInformation("Updating endpoint {Sip} in conference {ConferenceId}", sip, conferenceId);
            return _apiClient.UpdateDisplayNameForEndpointAsync(conferenceId, sip, request);
        }
    }
}