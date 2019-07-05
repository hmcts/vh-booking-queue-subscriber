using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Common.Security;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests
{
    public class VideoServiceTokenHandlerTests
    {
        [Test]
        public void should_add_authorization_header()
        {
            var memoryCache = new Mock<IMemoryCache>().Object;
            var tokenProviderMock = new Mock<IAzureTokenProvider>();
            var azureTokenProvider = tokenProviderMock.Object;
            new VideoServiceTokenHandler(
                new AzureAdConfiguration
                {
                    Authority = "auth",
                    ClientId = "id",
                    ClientSecret = "secret",
                    TenantId = "tenant",
                    VideoApiResourceId = "resourceid"
                }, memoryCache, azureTokenProvider);

            tokenProviderMock.Setup(x => x.GetAuthorisationResult(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}