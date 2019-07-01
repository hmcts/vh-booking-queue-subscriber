using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        public readonly IConfiguration Configuration;

        public ConfigLoader()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = configRootBuilder.Build();
        }
        
        public IOptions<AzureAdConfiguration> ReadAzureAdSettings()
        {
            return Options.Create(Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>());
        }
        
        public IOptions<HearingServicesConfiguration> ReadHearingServiceSettings()
        {
            return Options.Create(Configuration.GetSection("VhServices").Get<HearingServicesConfiguration>());
        }
    }
}