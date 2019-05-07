using BookingQueueSubscriber.Common.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Common.Security
{
    public class VideoApiTokenHandler : BaseServiceTokenHandler
    {
        public VideoApiTokenHandler(IOptions<AzureAdConfiguration> azureAdConfiguration,
            IOptions<HearingServicesConfiguration> hearingServicesConfiguration, IMemoryCache memoryCache,
            IAzureTokenProvider azureTokenProvider) : base(azureAdConfiguration, hearingServicesConfiguration, memoryCache,
            azureTokenProvider)
        {
        }
        
        protected override string TokenCacheKey => "VideoApiServiceToken";
        protected override string ClientResource => HearingServicesConfiguration.VideoApiResourceId;
    }
}