using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushNewConferenceAddedTests
    {
        [Test]
        public async Task Calls_Video_Web_With_Specified_Conference_Id_And_Path()
        {
            var handler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                .ReturnsAsync(response);
            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("http://video-web");
            var logger = new Mock<ILogger<VideoWebService>>();
            var videoWebService = new VideoWebService(client, logger.Object);
            var conferenceId = Guid.NewGuid();

            await videoWebService.PushNewConferenceAdded(conferenceId);

            handler.Protected().Verify("SendAsync", Times.Exactly(1),
                 ItExpr.Is<HttpRequestMessage>(
                request => request.Method == HttpMethod.Post && request.RequestUri.ToString().Contains(conferenceId.ToString())), ItExpr.IsAny<CancellationToken>());
        } 
    }
}
