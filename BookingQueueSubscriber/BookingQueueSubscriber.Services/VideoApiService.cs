using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Newtonsoft.Json;

namespace BookingQueueSubscriber.Services
{
    public interface IVideoApiService
    {
        Task BookNewConferenceAsync(BookNewConferenceRequest request);
        Task UpdateConferenceAsync(UpdateConferenceRequest request);
        Task DeleteConferenceAsync(Guid conferenceId);
        Task<ConferenceDto> GetConferenceByHearingRefId(Guid hearingRefId);
    }

    public class VideoApiService : IVideoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiUriFactory _apiUriFactory;

        public VideoApiService(HttpClient httpClient, HearingServicesConfiguration hearingServicesConfiguration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(hearingServicesConfiguration.VideoApiUrl);

            _apiUriFactory = new ApiUriFactory();
        }

        public async Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUriFactory.ConferenceEndpoints.BookNewConference, httpContent);
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
            var response = await _httpClient.DeleteAsync(_apiUriFactory.ConferenceEndpoints.DeleteConference(conferenceId));
            response.EnsureSuccessStatusCode();
        }

        public async Task<ConferenceDto> GetConferenceByHearingRefId(Guid hearingRefId)
        {
            var response = await _httpClient.GetAsync(_apiUriFactory.ConferenceEndpoints.GetConferenceByHearingRefId(hearingRefId));
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ConferenceDto>(content.Result);
        }
    }
}