namespace BookingQueueSubscriber.Security
{
    [ExcludeFromCodeCoverage]
    public class BookingsServiceTokenHandler : BaseServiceTokenHandler
    {
        protected override string TokenCacheKey => "BookingsApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.BookingsApiResourceId;

        public BookingsServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfigurationOptions,
            IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider) : base(azureAdConfiguration, servicesConfigurationOptions,
            memoryCache, azureTokenProvider)
        {
        }

    }
}
