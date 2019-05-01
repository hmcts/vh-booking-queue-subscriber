using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BookingQueueSubscriber;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

[assembly: WebJobsStartup(typeof(Startup))]
namespace BookingQueueSubscriber
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            builder.Services.AddScoped<IVideoApiService, VideoApiService>();
            
            RegisterMessageHandlers(builder.Services);
        }
        
        private static void RegisterMessageHandlers(IServiceCollection serviceCollection)
        {
            var eventHandlers = GetAllTypesOf<IMessageHandler>();
            
            foreach (var eventHandler in eventHandlers)
            {
                if (eventHandler.IsInterface || eventHandler.IsAbstract) continue;
                var serviceType = eventHandler.GetInterfaces()[0];
                serviceCollection.AddScoped(serviceType, eventHandler);
            }
        }
        
        private static IEnumerable<Type> GetAllTypesOf<T>()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            return runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => typeof(T).IsAssignableFrom(t));
        }
    }
}