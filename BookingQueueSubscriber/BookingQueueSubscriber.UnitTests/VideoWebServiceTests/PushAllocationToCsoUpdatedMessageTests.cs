using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushAllocationToCsoUpdatedMessageTests
    {
        [Test]
        public async Task Calls_Video_Web_With_Specified_Hearing_list()
        {
            var handler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                .ReturnsAsync(response);
            var client = new HttpClient(handler.Object);
            client.BaseAddress = new Uri("http://video-web");
            var logger = new Mock<ILogger<VideoWebService>>();
            var videoWebService = new VideoWebService(client, logger.Object);

            await videoWebService.PushAllocationToCsoUpdatedMessage(new AllocationHearingsToCsoRequest(){AllocatedCsoUserName = "username@mail.com", Hearings = buildHearingsRequest()});

            handler.Protected().Verify("SendAsync", Times.Exactly(1),
                 ItExpr.Is<HttpRequestMessage>(
                request => request.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
        }

        private static IList<HearingDetailRequest> buildHearingsRequest()
        {
            IList<HearingDetailRequest> list = new List<HearingDetailRequest>();
            HearingDetailRequest hearing = new HearingDetailRequest() {Judge = "Judge", CaseName = "CaseName", Time = new DateTimeOffset(2023,04,01,12,0,0,new TimeSpan())};
            list.Add(hearing);

            return list;
        }
    }
}
