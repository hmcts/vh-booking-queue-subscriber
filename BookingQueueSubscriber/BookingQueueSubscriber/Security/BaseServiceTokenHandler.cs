namespace BookingQueueSubscriber.Security;

[ExcludeFromCodeCoverage]
public abstract class BaseServiceTokenHandler : DelegatingHandler
{
    private readonly IMemoryCache _memoryCache;
    private readonly IAzureTokenProvider _azureTokenProvider;
    private readonly AzureAdConfiguration _azureAdConfiguration;
    protected readonly ServicesConfiguration ServicesConfiguration;
        
    protected abstract string TokenCacheKey { get; }
    protected abstract string ClientResource { get; }
        
    protected BaseServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
        IOptions<ServicesConfiguration> servicesConfigurationOptions,
        IMemoryCache memoryCache,
        IAzureTokenProvider azureTokenProvider)
    {
        _azureAdConfiguration = azureAdConfiguration.Value;
        ServicesConfiguration = servicesConfigurationOptions.Value;
        _memoryCache = memoryCache;
        _azureTokenProvider = azureTokenProvider;
    }
        
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _memoryCache.Get<string>(TokenCacheKey);
        if (string.IsNullOrEmpty(token))
        {
            var authenticationResult = await _azureTokenProvider.GetAuthorisationResult(_azureAdConfiguration.ClientId,
                _azureAdConfiguration.ClientSecret, ClientResource);
            token = authenticationResult.AccessToken;
            var tokenExpireDateTime = authenticationResult.ExpiresOn.DateTime.AddMinutes(-1);
            _memoryCache.Set(TokenCacheKey, token, tokenExpireDateTime);
        }

        request.Headers.Add("Authorization", $"Bearer {token}");
        return await base.SendAsync(request, cancellationToken);
    }
}