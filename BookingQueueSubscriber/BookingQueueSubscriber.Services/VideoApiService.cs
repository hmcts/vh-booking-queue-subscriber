using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services
{
    public interface IVideoApiService
    {
        Task BookNewConferenceAsync(BookNewConferenceRequest request);
        Task UpdateConferenceAsync(UpdateConferenceRequest request);
    }
    
    public class VideoApiService : IVideoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiUriFactory _apiUriFactory;

        public VideoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
    }
}