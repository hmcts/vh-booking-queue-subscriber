using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) =>
            builder.AddDependencyInjection(ConfigureServices);

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            var configLoader = new ConfigLoader();
            // need to check if bind works for both tests and host
            var adConfiguration = configLoader.Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>() ?? BuildAdConfiguration(configLoader);
            services.AddSingleton(adConfiguration);

            var hearingServicesConfiguration =
                configLoader.Configuration.GetSection("VhServices").Get<HearingServicesConfiguration>() ??
                BuildHearingServicesConfiguration(configLoader);
            services.AddSingleton(hearingServicesConfiguration);

            services.AddScoped<IAzureTokenProvider, AzureTokenProvider>();
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddScoped<VideoServiceTokenHandler>();
            services.AddLogging(builder => { builder.SetMinimumLevel(LogLevel.Debug); });

            if (hearingServicesConfiguration.EnableVideoApiStub)
            {
                services.AddScoped<IVideoApiService, VideoApiServiceFake>();
            }
            else
            {
                services.AddHttpClient<IVideoApiService, VideoApiService>()
                    .AddHttpMessageHandler<VideoServiceTokenHandler>();
            }

            RegisterMessageHandlers(services);
        }

        private static HearingServicesConfiguration BuildHearingServicesConfiguration(ConfigLoader configLoader)
        {
            var values = configLoader.Configuration.GetSection("Values");
            var hearingServicesConfiguration = new HearingServicesConfiguration();
            values.GetSection("VhServices").Bind(hearingServicesConfiguration);
            return hearingServicesConfiguration;
        }

        private static AzureAdConfiguration BuildAdConfiguration(ConfigLoader configLoader)
        {
            var values = configLoader.Configuration.GetSection("Values");
            var azureAdConfiguration = new AzureAdConfiguration();
            values.GetSection("AzureAd").Bind(azureAdConfiguration);
            return azureAdConfiguration;
        }

        private static void RegisterMessageHandlers(IServiceCollection serviceCollection)
        {
            var messageHandlers = GetAllTypesOf(typeof(IMessageHandler<>)).ToList();
            foreach (var messageHandler in messageHandlers)
            {
                var serviceType = messageHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, messageHandler);
            }
        }

        private static IEnumerable<Type> GetAllTypesOf(Type @interface)
        {
            return @interface.Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == @interface));
        }
    }
}