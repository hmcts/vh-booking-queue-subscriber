using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "local.settings.json"), true, true)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true, true)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "function.json"), true, true)
                .AddAksKeyVaultSecretProvider(vhInfraCore)
                .AddAksKeyVaultSecretProvider(vhBookingQueueSubscriber)
                .AddEnvironmentVariables();

            Configuration = configRootBuilder.Build();
        }
    }

    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAppsettingsFile(
            this IConfigurationBuilder configurationBuilder,
            FunctionsHostBuilderContext context,
            bool useEnvironment = false
        )
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var environmentSection = string.Empty;

            if (useEnvironment)
            {
                environmentSection = $".{context.EnvironmentName}";
            }

            configurationBuilder.AddJsonFile(
                path: Path.Combine(context.ApplicationRootPath, $"appsettings{environmentSection}.json"),
                optional: true,
                reloadOnChange: false);

            return configurationBuilder;
        }
    }
}