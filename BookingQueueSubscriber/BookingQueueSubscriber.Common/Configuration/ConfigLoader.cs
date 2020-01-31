using Microsoft.Extensions.Configuration;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        public readonly IConfiguration Configuration;

        public ConfigLoader()
        {
            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = configRootBuilder.Build();
        }
    }
}