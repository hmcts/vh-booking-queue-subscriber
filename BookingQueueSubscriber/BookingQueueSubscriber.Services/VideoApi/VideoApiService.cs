using System.Net;
using VideoApi.Contract.Requests;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public class VideoApiService : IVideoApiService
    {
        private readonly IVideoApiClient _apiClient;
        private readonly ILogger<VideoApiService> _logger;

        public VideoApiService(IVideoApiClient apiClient, ILogger<VideoApiService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public Task<ConferenceDetailsResponse> BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _logger.LogInformation("Booking new conference for hearing {Hearing}", request.HearingRefId);
            return _apiClient.BookNewConferenceAsync(request);
        }

        public Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            _logger.LogInformation("Updating conference with hearing id {Hearing}", request.HearingRefId);
            return _apiClient.UpdateConferenceAsync(request);
        }

        public Task DeleteConferenceAsync(Guid conferenceId)
        {
            _logger.LogInformation("Deleting conference id {ConferenceId}", conferenceId);
            return _apiClient.RemoveConferenceAsync(conferenceId);
        }

        public async Task<ConferenceDetailsResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false)
        {
            _logger.LogInformation("Getting conference by hearing ref id {HearingId}", hearingRefId);
            var request = new GetConferencesByHearingIdsRequest { HearingRefIds =  new Guid[]{hearingRefId}, IncludeClosed = includeClosed };
            var conferences = await _apiClient.GetConferenceDetailsByHearingRefIdsAsync(request);
            return conferences.FirstOrDefault();
        }

        public Task<ICollection<EndpointResponse>> GetEndpointsForConference(Guid conferenceId)
        {
            _logger.LogInformation("Getting endpoints by conference id {ConferenceId}", conferenceId);
            return _apiClient.GetEndpointsForConferenceAsync(conferenceId);
        }

        public Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            _logger.LogInformation("Adding participants to conference {ConferenceId}", conferenceId);
            return _apiClient.AddParticipantsToConferenceAsync(conferenceId, request);
        }

        public Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            _logger.LogInformation("Removing participant {ParticipantId} from conference {ConferenceId}", participantId,
                conferenceId);
            return _apiClient.RemoveParticipantFromConferenceAsync(conferenceId, participantId);
        }

        public Task UpdateConferenceParticipantsAsync(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            _logger.LogInformation("Updating participants in conference {ConferenceId} with request: {Request}", conferenceId, request);
            return _apiClient.UpdateConferenceParticipantsAsync(conferenceId, request);
        }

        public Task UpdateParticipantDetails(Guid conferenceId, Guid participantId,
            UpdateParticipantRequest request)
        {
            _logger.LogInformation("Updating participant {ParticipantId} in conference {ConferenceId}", participantId,
                conferenceId);
            return _apiClient.UpdateParticipantDetailsAsync(conferenceId, participantId, request);
        }

        public Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
        {
            _logger.LogInformation("Adding endpoint to conference: {ConferenceId}", conferenceId);
            return _apiClient.AddEndpointToConferenceAsync(conferenceId, request);
        }

        public Task RemoveEndpointFromConference(Guid conferenceId, string sip)
        {
            _logger.LogInformation("Removing endpoint {Sip} from conference {ConferenceId}", sip, conferenceId);
            return _apiClient.RemoveEndpointFromConferenceAsync(conferenceId, sip);
        }

        public Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request)
        {
            _logger.LogInformation("Updating endpoint {Sip} in conference {ConferenceId}", sip, conferenceId);
            return _apiClient.UpdateEndpointInConferenceAsync(conferenceId, sip, request);
        }

        public Task CloseConsultation(Guid conferenceId, Guid participantId)
        {
            _logger.LogInformation("Closing consultation for conference {ConferenceId}", conferenceId);
            return _apiClient.LeaveConsultationAsync(new LeaveConsultationRequest{ConferenceId = conferenceId, ParticipantId = participantId});
        }

        private Task UpdateParticipantUsername(Guid participantId, string username)
        {
            _logger.LogInformation("Updating username for participant {ParticipantId}", participantId);
            return _apiClient.UpdateParticipantUsernameAsync(participantId, new UpdateParticipantUsernameRequest { Username = username });
        }

        public async Task UpdateParticipantUsernameWithPolling(Guid hearingId, string username, string contactEmail)
        {
            var pollCount = 0;
            
            ConferenceDetailsResponse conferenceResponse;
            do {
                conferenceResponse = await PollForConferenceDetails(); 
                pollCount++;
            } while (conferenceResponse == null);

            var participant = conferenceResponse.Participants.Single(x => x.ContactEmail == contactEmail);
            await UpdateParticipantUsername(participant.Id, username);
            
            async Task<ConferenceDetailsResponse> PollForConferenceDetails()
            {
                try
                {
                    return await GetConferenceByHearingRefId(hearingId, true);
                }
                catch (VideoApiException e)
                {
                    if(pollCount >= 3) 
                        throw;
                
                    if (e.StatusCode == (int) HttpStatusCode.NotFound)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        return null;
                    }

                    throw;
                }
            }
        }

        public Task<ICollection<ConferenceDetailsResponse>> GetConferencesByHearingRefIdsAsync(
            GetConferencesByHearingIdsRequest request)
        {
            return _apiClient.GetConferenceDetailsByHearingRefIdsAsync(request);
        }
    }
}