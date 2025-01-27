namespace BookingQueueSubscriber.Security
{
    [ExcludeFromCodeCoverage]
    public class UserServiceTokenHandler : BaseServiceTokenHandler
    {
        protected override string TokenCacheKey => "UserApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.UserApiResourceId;

        public UserServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfigurationOptions,
            IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider) : base(azureAdConfiguration, servicesConfigurationOptions,
            memoryCache, azureTokenProvider)
        {
        }
    }
}
