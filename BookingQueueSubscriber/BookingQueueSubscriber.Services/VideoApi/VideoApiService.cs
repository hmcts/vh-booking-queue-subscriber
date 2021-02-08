using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.VideoApi
{
    public class VideoApiService : IVideoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _log;
        private readonly ApiUriFactory _apiUriFactory;

        public VideoApiService(HttpClient httpClient, ServicesConfiguration servicesConfiguration,
            ILoggerFactory factory)
        {
            _httpClient = httpClient;
            _log = factory.CreateLogger<VideoApiService>();
            _httpClient.BaseAddress = new Uri(servicesConfiguration.VideoApiUrl);

            _apiUriFactory = new ApiUriFactory();
        }

        public async Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _log.LogInformation("Booking new conference for hearing {Hearing}", request.HearingRefId);
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response =
                await _httpClient.PostAsync(_apiUriFactory.ConferenceEndpoints.BookNewConference, httpContent);
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Successfully booked a new conference for {Hearing}", request.HearingRefId);
        }

        public async Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            _log.LogInformation("Attempting to update conference for hearing {Hearing}", request.HearingRefId);
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_apiUriFactory.ConferenceEndpoints.UpdateConference, httpContent);
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Updated conference for hearing {Hearing}", request.HearingRefId);
        }

        public async Task DeleteConferenceAsync(Guid conferenceId)
        {
            _log.LogInformation("Attempting to delete conference for {Conference}", conferenceId);
            var response = await _httpClient
                .DeleteAsync(_apiUriFactory.ConferenceEndpoints.DeleteConference(conferenceId));
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Deleted {Conference}", conferenceId);
        }

        public async Task<ConferenceResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false)
        {
            _log.LogInformation("Getting conference by hearing ref id {HearingRefId}", hearingRefId);
            var uriString = _apiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId, true);
            var response = await _httpClient.GetAsync(uriString);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceResponse>(content);
        }

        public async Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            _log.LogInformation("Adding participants to conference {Conference}", conferenceId);
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                    _apiUriFactory.ParticipantsEndpoints.AddParticipantsToConference(conferenceId), httpContent);
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Added participants to conference {Conference}", conferenceId);
        }

        public async Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            _log.LogInformation("Removing participant {Participant} from conference {Conference}", participantId,
                conferenceId);
            var response = await _httpClient.DeleteAsync(
                _apiUriFactory.ParticipantsEndpoints.RemoveParticipantFromConference(conferenceId, participantId));
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Removed participant {Participant} from conference {Conference}", participantId,
                conferenceId);
        }

        public async Task UpdateParticipantDetails(Guid conferenceId, Guid participantId,
            UpdateParticipantRequest request)
        {
            _log.LogInformation("Updating participant {Participant} in conference {Conference}", participantId,
                conferenceId);
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(_apiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(
                conferenceId, participantId), httpContent);
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Updated participant {Participant} in conference {Conference}", participantId,
                conferenceId);
        }

        public async Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
        {
            _log.LogInformation("Adding endpoint to conference {Conference}", conferenceId);
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                _apiUriFactory.EndpointForJvsEndpoints.AddEndpoint(conferenceId), httpContent);
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Added endpoint to conference {Conference}", conferenceId);
        }

        public async Task RemoveEndpointFromConference(Guid conferenceId, string sip)
        {
            _log.LogInformation("Removing endpoint to conference: {Conference}, Endpoint: {Sip}", conferenceId, sip);
            var response = await _httpClient.DeleteAsync(
                _apiUriFactory.EndpointForJvsEndpoints.RemoveEndpoint(conferenceId, sip));
            response.EnsureSuccessStatusCode();
            _log.LogInformation("Removed endpoint to conference: {Conference}, Endpoint: {Sip}", conferenceId, sip);
        }

        public async Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(_apiUriFactory.EndpointForJvsEndpoints.UpdateEndpoint(conferenceId,
                    sip), 
                httpContent);
            response.EnsureSuccessStatusCode();
        }
    }
}