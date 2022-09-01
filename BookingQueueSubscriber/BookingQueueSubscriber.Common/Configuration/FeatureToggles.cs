using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace BookingQueueSubscriber.Common.Configuration
{
    public interface IFeatureToggles
    {
        public bool SsprToggle();
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly User _user;
        private const string LdUser = "vh-booking-queue-subscriber";
        private const string SsprToggleKey = "sspr";
        public FeatureToggles(string sdkKey)
        {
            _ldClient = new LdClient(sdkKey);
            _user = User.WithKey(LdUser);
        }

        public bool SsprToggle() => _ldClient.BoolVariation(SsprToggleKey, _user);
    }
}
