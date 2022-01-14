using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Client;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.VideoApiServiceTests
{
    public class BookNewConferenceTests
    {
        [Test]
        public async Task Returns_Conference_Details_Response()
        {
            var videoApiClientMock = new Mock<IVideoApiClient>();
            var loggerMock = new Mock<ILogger<VideoApiService>>();
            var videoApiService = new VideoApiService(videoApiClientMock.Object, loggerMock.Object);
            var bookNewConferenceRequest = new BookNewConferenceRequest();
            var conferenceDetailsResponse = new ConferenceDetailsResponse();
            videoApiClientMock.Setup(x => x.BookNewConferenceAsync(It.Is<BookNewConferenceRequest>(x => x == bookNewConferenceRequest))).ReturnsAsync(conferenceDetailsResponse);

            var result = await videoApiService.BookNewConferenceAsync(bookNewConferenceRequest);

            Assert.AreEqual(conferenceDetailsResponse, result);
        }
    }
}
