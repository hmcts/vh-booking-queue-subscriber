using System;
using Microsoft.Extensions.DependencyInjection;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; }

        static ServiceProviderFactory()
        {
            ServiceCollection sc = new ServiceCollection();
            Startup.ConfigureServices(sc);
            ServiceProvider = sc.BuildServiceProvider();
        }
    }
}