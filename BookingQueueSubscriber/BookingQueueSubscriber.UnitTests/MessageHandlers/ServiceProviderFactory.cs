using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; }

        static ServiceProviderFactory()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            ServiceCollection sc = new ServiceCollection();
            Startup.RegisterServices(sc, configuration);
            ServiceProvider = sc.BuildServiceProvider();
        }
    }
}