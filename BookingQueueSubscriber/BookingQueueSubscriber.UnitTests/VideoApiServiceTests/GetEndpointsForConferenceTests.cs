using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.VideoApiServiceTests
{
    public class GetEndpointsForConferenceTests
    {
        [Test]
        public async Task should_get_endpoints_for_a_conference()
        {
            // arrange
            var conferenceId = Guid.NewGuid();
            var endpoints = Builder<EndpointResponse>.CreateListOfSize(3).Build().ToList();
            
            var videoApiClientMock = new Mock<IVideoApiClient>();
            var loggerMock = new Mock<ILogger<VideoApiService>>();
            var videoApiService = new VideoApiService(videoApiClientMock.Object, loggerMock.Object);
            
            videoApiClientMock.Setup(x => x.GetEndpointsForConferenceAsync(conferenceId)).ReturnsAsync(endpoints);
            
            // act
            var result =await  videoApiService.GetEndpointsForConference(conferenceId);
            
            // assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(endpoints);
        }
    }
}