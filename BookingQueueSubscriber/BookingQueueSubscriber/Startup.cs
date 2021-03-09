using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookingQueueSubscriber;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VH.Core.Configuration;
using VideoApi.Client;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            const string vhInfraCore = "/mnt/secrets/vh-infra-core";
            const string vhBookingQueueSubscriber = "/mnt/secrets/vh-booking-queue-subscriber";

            var context = builder.GetContext();
            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.json"), true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), true)
                .AddAksKeyVaultSecretProvider(vhInfraCore)
                .AddAksKeyVaultSecretProvider(vhBookingQueueSubscriber)
                .AddUserSecrets("F6705640-D918-4180-B98A-BAB7ADAA4817")
                .AddEnvironmentVariables();

            base.ConfigureAppConfiguration(builder);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var context = builder.GetContext();
            RegisterServices(builder.Services, context.Configuration);
        }

        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            services.AddSingleton<IMemoryCache>(memoryCache);
            services.Configure<AzureAdConfiguration>(options =>
            {
                configuration.GetSection("AzureAd").Bind(options);
            });
            services.Configure<ServicesConfiguration>(options =>
            {
                configuration.GetSection("VhServices").Bind(options);
            });

            var serviceConfiguration = new ServicesConfiguration();
            configuration.GetSection("VhServices").Bind(serviceConfiguration);

            services.AddScoped<IAzureTokenProvider, AzureTokenProvider>();
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddTransient<VideoServiceTokenHandler>();
            services.AddLogging(builder => 
                builder.AddApplicationInsights(configuration["ApplicationInsights:InstrumentationKey"])
            );
            RegisterMessageHandlers(services);

            var container = services.BuildServiceProvider();

            if (serviceConfiguration.EnableVideoApiStub)
            {
                services.AddScoped<IVideoApiService, VideoApiServiceFake>();
            }
            else
            {
                services.AddScoped<IVideoApiService, VideoApiService>();
                services.AddHttpClient<IVideoApiClient, VideoApiClient>()
                    .AddHttpMessageHandler(() => container.GetService<VideoServiceTokenHandler>())
                    .AddTypedClient(httpClient =>
                    {
                        var client = VideoApiClient.GetClient(httpClient);
                        client.BaseUrl = serviceConfiguration.VideoApiUrl;
                        client.ReadResponseAsString = true;
                        return (IVideoApiClient)client;
                    });
            }
        }

        private void RegisterMessageHandlers(IServiceCollection serviceCollection)
        {
            var messageHandlers = GetAllTypesOf(typeof(IMessageHandler<>)).ToList();
            foreach (var messageHandler in messageHandlers)
            {
                var serviceType = messageHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, messageHandler);
            }
        }

        private IEnumerable<Type> GetAllTypesOf(Type i)
        {
            return i.Assembly.GetTypes().Where(t =>
                t.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == i));
        }
    }
}