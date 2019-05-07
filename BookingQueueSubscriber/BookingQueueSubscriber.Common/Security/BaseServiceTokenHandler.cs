using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Common.Security
{
    public abstract class BaseServiceTokenHandler : DelegatingHandler
    {
        private readonly IAzureTokenProvider _azureTokenProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly AzureAdConfiguration _azureAdConfiguration;
        protected readonly HearingServicesConfiguration HearingServicesConfiguration;
        
        protected abstract string TokenCacheKey { get; }
        protected abstract string ClientResource { get; }
        
        protected BaseServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<HearingServicesConfiguration> hearingServicesConfiguration, IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider)
        {
            _azureAdConfiguration = azureAdConfiguration.Value;
            HearingServicesConfiguration = hearingServicesConfiguration.Value;
            _memoryCache = memoryCache;
            _azureTokenProvider = azureTokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _memoryCache.Get<string>(TokenCacheKey);
            if (string.IsNullOrEmpty(token))
            {
                var authenticationResult = _azureTokenProvider.GetAuthorisationResult(_azureAdConfiguration.ClientId,
                    _azureAdConfiguration.ClientSecret, ClientResource);
                token = authenticationResult.AccessToken;
                var tokenExpireDateTime = authenticationResult.ExpiresOn.DateTime.AddMinutes(-1);
                _memoryCache.Set(TokenCacheKey, token, tokenExpireDateTime);
            }
            
            request.Headers.Add("Authorization", $"Bearer {token}");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}