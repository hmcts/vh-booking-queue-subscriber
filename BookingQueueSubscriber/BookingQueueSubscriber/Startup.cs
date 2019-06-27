using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    internal class Startup : IWebJobsStartup
    {
        private static HearingServicesConfiguration _hearingServicesConfiguration;

        public void Configure(IWebJobsBuilder builder) =>
            builder.AddDependencyInjection(ConfigureServices);
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            RegisterSettings(services);
            
            services.AddScoped<IAzureTokenProvider, AzureAzureTokenProvider>();
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddTransient<VideoServiceTokenHandler>();

            services.AddHttpClient<IVideoApiService, VideoApiService>()
                .AddHttpMessageHandler<VideoServiceTokenHandler>()
                .AddTypedClient(httpClient =>
                {
                    httpClient.BaseAddress = new Uri(_hearingServicesConfiguration.VideoApiUrl);
                    return new VideoApiService(httpClient);
                });

            RegisterMessageHandlers(services);
        }
        
        private static void RegisterSettings(IServiceCollection services)
        {
            var configLoader = new ConfigLoader();
            services.Configure<AzureAdConfiguration>(options => configLoader.ConfigRoot.Bind("AzureAd",options));
            services.Configure<HearingServicesConfiguration>(options =>
                configLoader.ConfigRoot.Bind("VhServices", options));
            _hearingServicesConfiguration = configLoader.ConfigRoot.Get<HearingServicesConfiguration>();
        }
        
        private static void RegisterMessageHandlers(IServiceCollection serviceCollection)
        {
            var messageHandlers = GetAllTypesOf<IMessageHandler>().ToList();
            foreach (var messageHandler in messageHandlers)
            {
                if (messageHandler.IsInterface || messageHandler.IsAbstract) continue;
                var serviceType = messageHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, messageHandler);
            }
        }
        
        private static IEnumerable<Type> GetAllTypesOf<T>()
        {
            var @interface = typeof (T);
            return @interface.IsInterface
                ? AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => !type.IsInterface
                                   && !type.IsAbstract 
                                   && type.GetInterfaces().Contains(@interface))
                : new Type[] {};
        }
    }
}