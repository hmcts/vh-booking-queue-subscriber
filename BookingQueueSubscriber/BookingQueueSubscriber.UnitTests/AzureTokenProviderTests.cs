using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;

namespace BookingQueueSubscriber.UnitTests
{
    public class AzureTokenProviderTests
    {
        [Test]
        public void should_get_access_token()
        {
            var azureAdConfigurationOptions = Options.Create(new AzureAdConfiguration { TenantId = "teanantid" });
            var azureTokenProvider = new AzureTokenProvider(azureAdConfigurationOptions);
            Assert.Throws<AggregateException>(() => azureTokenProvider.GetClientAccessToken("1234", "1234", "1234"));
        }
    }
}