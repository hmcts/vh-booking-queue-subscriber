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
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) =>
            builder.AddDependencyInjection(ConfigureServices);

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            var configLoader = new ConfigLoader();
            var adConfiguration = configLoader.Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
            services.AddSingleton(adConfiguration);
            var hearingServicesConfiguration = configLoader.Configuration.GetSection("VhServices").Get<HearingServicesConfiguration>();
            services.AddSingleton(hearingServicesConfiguration);

            services.AddScoped<IAzureTokenProvider, AzureAzureTokenProvider>();
            services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddScoped<VideoServiceTokenHandler>();

            services.AddHttpClient<IVideoApiService, VideoApiService>()
                .AddHttpMessageHandler<VideoServiceTokenHandler>();

            RegisterMessageHandlers(services);
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