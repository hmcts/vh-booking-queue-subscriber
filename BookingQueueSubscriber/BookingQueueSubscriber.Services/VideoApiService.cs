using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace BookingQueueSubscriber.Services
{
    public interface IVideoApiService
    {
        Task BookNewConferenceAsync(BookNewConferenceRequest request);
    }
    
    public class VideoApiService : IVideoApiService
    {
        private readonly ApiUriFactory _apiUriFactory;
        private readonly VideoApiTokenHandler _apiTokenHandler;
        private readonly HearingServicesConfiguration _hearingServices;

        public VideoApiService()
        {
            _apiUriFactory = new ApiUriFactory();
            
            // configure message handler for video api
            var configLoader = new ConfigLoader();
            _hearingServices = configLoader.ReadHearingServiceSettings().Value;
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            
            _apiTokenHandler = new VideoApiTokenHandler(configLoader.ReadAzureAdSettings(),
                configLoader.ReadHearingServiceSettings(), memoryCache,
                new AzureAzureTokenProvider(configLoader.ReadAzureAdSettings()));
        }
        
        public async Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            
            using (var httpClient = new HttpClient(_apiTokenHandler))
            {
                httpClient.BaseAddress = new Uri(_hearingServices.VideoApiUrl);
                var response =
                    await httpClient.PostAsync(_apiUriFactory.ConferenceEndpoints.BookNewConference, httpContent);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}