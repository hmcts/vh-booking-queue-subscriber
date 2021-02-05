using Microsoft.Extensions.Configuration;
using VH.Core.Configuration;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        public readonly IConfiguration Configuration;

        public ConfigLoader()
        {
            const string mountPath = "/mnt/secrets/vh-booking-queue-subscriber";

            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
                .AddAksKeyVaultSecretProvider(mountPath);

            Configuration = configRootBuilder.Build();
        }
    }
}