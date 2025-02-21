namespace BookingQueueSubscriber.Security
{
    [ExcludeFromCodeCoverage]
    public class NotificationServiceTokenHandler : BaseServiceTokenHandler
    {
        protected override string TokenCacheKey => "NotificationApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.NotificationApiResourceId;

        public NotificationServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfigurationOptions,
            IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider) : base(azureAdConfiguration, servicesConfigurationOptions,
            memoryCache, azureTokenProvider)
        {
        }
    }
}
