using Microsoft.Extensions.Configuration;
using VH.Core.Configuration;

namespace BookingQueueSubscriber.Common.Configuration
{
    public class ConfigLoader
    {
        public readonly IConfiguration Configuration;

        public ConfigLoader()
        {
            const string vhInfraCore = "/mnt/secrets/vh-infra-core";
            const string vhBookingQueueSubscriber = "/mnt/secrets/vh-booking-queue-subscriber";

            var configRootBuilder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true, true)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .AddAksKeyVaultSecretProvider(vhInfraCore)
                .AddAksKeyVaultSecretProvider(vhBookingQueueSubscriber);

            Configuration = configRootBuilder.Build();
        }
    }
}