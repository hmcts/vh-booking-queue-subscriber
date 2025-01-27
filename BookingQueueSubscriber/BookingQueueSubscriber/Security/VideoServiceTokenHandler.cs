namespace BookingQueueSubscriber.Security
{
    [ExcludeFromCodeCoverage]
    public class VideoServiceTokenHandler : BaseServiceTokenHandler
    {
        protected override string TokenCacheKey => "VideoApiServiceToken";
        protected override string ClientResource => ServicesConfiguration.VideoApiResourceId;

        public VideoServiceTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<ServicesConfiguration> servicesConfigurationOptions,
            IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider) : base(azureAdConfiguration, servicesConfigurationOptions,
            memoryCache, azureTokenProvider)
        {
        }
    }
}