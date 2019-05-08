using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Services
{
    public interface IVideoApiService
    {
        Task BookNewConferenceAsync(BookNewConferenceRequest request);
    }
    
    public class VideoApiService : IVideoApiService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAzureTokenProvider _azureTokenProvider;
        private readonly AzureAdConfiguration _azureAdConfiguration;
        private readonly ApiUriFactory _apiUriFactory;
        private readonly VideoApiTokenHandler _apiTokenHandler;
        private readonly HearingServicesConfiguration _hearingServices;

        private string _tokenCacheKey => "VideoApiServiceToken";

        public VideoApiService(IMemoryCache memoryCache, IAzureTokenProvider azureTokenProvider,
            IOptions<HearingServicesConfiguration> hearingServicesConfig,
            IOptions<AzureAdConfiguration> azureAdConfiguration)
        {
            _memoryCache = memoryCache;
            _azureTokenProvider = azureTokenProvider;
            _apiUriFactory = new ApiUriFactory();
            _hearingServices = hearingServicesConfig.Value;
            _azureAdConfiguration = azureAdConfiguration.Value;
        }

        public async Task BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var bearerToken = GetBearerToken();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                httpClient.BaseAddress = new Uri(_hearingServices.VideoApiUrl);
                var response =
                    await httpClient.PostAsync(_apiUriFactory.ConferenceEndpoints.BookNewConference, httpContent);
                response.EnsureSuccessStatusCode();
            }
        }

        private string GetBearerToken()
        {
            var token = _memoryCache.Get<string>(_tokenCacheKey);
            if (!string.IsNullOrEmpty(token)) return token;
            
            var authenticationResult = _azureTokenProvider.GetAuthorisationResult(_azureAdConfiguration.ClientId,
                _azureAdConfiguration.ClientSecret, _hearingServices.VideoApiResourceId);
            token = authenticationResult.AccessToken;
            var tokenExpireDateTime = authenticationResult.ExpiresOn.DateTime.AddMinutes(-1);
            _memoryCache.Set(_tokenCacheKey, token, tokenExpireDateTime);

            return token;
        }
    }
}