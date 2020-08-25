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

        public VideoApiService(HttpClient httpClient, HearingServicesConfiguration hearingServicesConfiguration,
            ILoggerFactory factory)
        {
            _httpClient = httpClient;
            _log = factory.CreateLogger<VideoApiService>();
            _httpClient.BaseAddress = new Uri(hearingServicesConfiguration.VideoApiUrl);

            _apiUriFactory = new ApiUriFactory();
        }

        public async Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _log.LogInformation($"Booking new conference for {request.HearingRefId}");
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response =
                await _httpClient.PostAsync(_apiUriFactory.ConferenceEndpoints.BookNewConference, httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_apiUriFactory.ConferenceEndpoints.UpdateConference, httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteConferenceAsync(Guid conferenceId)
        {
            _log.LogInformation($"Deleting conference for {conferenceId}");
            var response = await _httpClient
                .DeleteAsync(_apiUriFactory.ConferenceEndpoints.DeleteConference(conferenceId));
            response.EnsureSuccessStatusCode();
        }

        public async Task<ConferenceResponse> GetConferenceByHearingRefId(Guid hearingRefId)
        {
            _log.LogInformation($"Getting conference by hearing ref id {hearingRefId}");
            var uriString = _apiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId);
            var response = await _httpClient.GetAsync(uriString);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceResponse>(content);
        }

        public async Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            _log.LogInformation($"Adding participants to conference: {conferenceId}");
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(
                    _apiUriFactory.ParticipantsEndpoints.AddParticipantsToConference(conferenceId), httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
        {
            var response = await _httpClient.DeleteAsync(
                    _apiUriFactory.ParticipantsEndpoints.RemoveParticipantFromConference(conferenceId, participantId));
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateParticipantDetails(Guid conferenceId, Guid participantId,
            UpdateParticipantRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(_apiUriFactory.ParticipantsEndpoints.UpdateParticipantDetails(
                conferenceId, participantId), httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
        {
            _log.LogInformation($"Adding endpoint to conference: {conferenceId}");
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                _apiUriFactory.EndpointForJvsEndpoints.AddEndpoint(conferenceId), httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveEndpointFromConference(Guid conferenceId, Guid endpointId)
        {
            _log.LogInformation($"Removing endpoint to conference: {conferenceId}, Endpoint: {endpointId}");
            var response = await _httpClient.DeleteAsync(
                _apiUriFactory.EndpointForJvsEndpoints.RemoveEndpoint(conferenceId, endpointId));
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateEndpointInConference(Guid conferenceId, Guid endpointId, UpdateEndpointRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(_apiUriFactory.EndpointForJvsEndpoints.UpdateEndpoint(
                conferenceId, endpointId), httpContent);
            response.EnsureSuccessStatusCode();
        }
    }
}