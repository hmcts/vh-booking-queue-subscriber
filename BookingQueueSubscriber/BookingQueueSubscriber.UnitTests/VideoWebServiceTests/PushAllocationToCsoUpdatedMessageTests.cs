﻿using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using BookingQueueSubscriber.Services.VideoWeb.Models;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushAllocationToCsoUpdatedMessageTests
    {
        [Test]
        public async Task Calls_Video_Web_With_Specified_Hearing_list()
        {
            var handler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("http://video-web");
            var logger = new Mock<ILogger<VideoWebService>>();
            var videoWebService = new VideoWebService(client, logger.Object);

            await videoWebService.PushAllocationToCsoUpdatedMessage(new HearingAllocationNotificationRequest()
            {
                AllocatedCsoUserName = "username@mail.com", AllocatedCsoUserId = Guid.NewGuid(),
                AllocatedCsoFullName = "CSO Name", ConferenceIds = new List<Guid>() { Guid.NewGuid() }
            });

            handler.Protected().Verify("SendAsync", Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(
                    request => request.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
        }
    }
}
