using System.Diagnostics.CodeAnalysis;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace BookingQueueSubscriber.Common.Configuration
{
    public interface IFeatureToggles
    {
        bool UsePostMay2023Template();
    }

    [ExcludeFromCodeCoverage]
    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-booking-queue-subscriber";
        private const string NewNotifyTemplatesToggleKey = "notify-post-may-2023-templates";

        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = LaunchDarkly.Sdk.Server.Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            _ldClient = new LdClient(config);
        }

        public bool UsePostMay2023Template()
        {
            if (!_ldClient.Initialized)
            {
                throw new InvalidOperationException("LaunchDarkly client not initialized");
            }

            return _ldClient.BoolVariation(NewNotifyTemplatesToggleKey, _context);
        }
    }
}
