﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber
{
    public class NotificationServiceTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAzureTokenProvider _azureTokenProvider;
        private readonly AzureAdConfiguration _azureAdConfiguration;
        private readonly ServicesConfiguration _servicesConfiguration;

        private const string TokenCacheKey = "NotificationApiServiceToken";

        public NotificationServiceTokenHandler(
            IOptions<AzureAdConfiguration> azureAdConfigurationOptions,
            IOptions<ServicesConfiguration> servicesConfigurationOptions,
            IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider)
        {
            _azureAdConfiguration = azureAdConfigurationOptions.Value;
            _servicesConfiguration = servicesConfigurationOptions.Value;
            _memoryCache = memoryCache;
            _azureTokenProvider = azureTokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _memoryCache.Get<string>(TokenCacheKey);
            if (string.IsNullOrEmpty(token))
            {
                var authenticationResult = _azureTokenProvider.GetAuthorisationResult(_azureAdConfiguration.ClientId,
                    _azureAdConfiguration.ClientSecret, _servicesConfiguration.NotificationApiResourceId);
                token = authenticationResult.AccessToken;
                var tokenExpireDateTime = authenticationResult.ExpiresOn.DateTime.AddMinutes(-1);
                _memoryCache.Set(TokenCacheKey, token, tokenExpireDateTime);
            }

            request.Headers.Add("Authorization", $"Bearer {token}");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
