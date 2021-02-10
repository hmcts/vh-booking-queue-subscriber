using System;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class AzureTokenProviderTests
    {
        [Test]
        public void should_get_access_token()
        {
            var azureTokenProvider = new AzureTokenProvider(new AzureAdConfiguration{ TenantId = "teanantid"});
            Assert.Throws<AggregateException>(() => azureTokenProvider.GetClientAccessToken("1234", "1234", "1234"));
        }
    }
}