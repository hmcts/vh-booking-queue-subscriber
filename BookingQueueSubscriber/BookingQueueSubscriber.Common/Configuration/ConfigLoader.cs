using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        private readonly IConfigurationRoot _configRoot;

        public ConfigLoader()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("F6705640-D918-4180-B98A-BAB7ADAA4817");
            _configRoot = configRootBuilder.Build();
        }
        
        public IOptions<AzureAdConfiguration> ReadAzureAdSettings()
        {
            return Options.Create(_configRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
        }
        
        public IOptions<HearingServicesConfiguration> ReadHearingServiceSettings()
        {
            return Options.Create(_configRoot.GetSection("VhServices").Get<HearingServicesConfiguration>());
        }
    }
}