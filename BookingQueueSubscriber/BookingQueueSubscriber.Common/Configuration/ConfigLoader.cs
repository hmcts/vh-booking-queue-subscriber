using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        public readonly IConfigurationRoot ConfigRoot;

        public ConfigLoader()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            ConfigRoot = configRootBuilder.Build();
        }
        
        public IOptions<AzureAdConfiguration> ReadAzureAdSettings()
        {
            return Options.Create(ConfigRoot.GetSection("AzureAd").Get<AzureAdConfiguration>());
        }
        
        public IOptions<HearingServicesConfiguration> ReadHearingServiceSettings()
        {
            return Options.Create(ConfigRoot.GetSection("VhServices").Get<HearingServicesConfiguration>());
        }
    }
}