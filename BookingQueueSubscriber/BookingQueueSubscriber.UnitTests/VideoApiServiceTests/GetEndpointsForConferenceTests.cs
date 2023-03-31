using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.VideoApiServiceTests
{
    public class GetEndpointsForConferenceTests
    {
        [Test]
        public async Task should_get_endpoints_for_conference_using_mock_http()
        {
            // arrange
            var baseAddress = "http://video-api";
            var conferenceId = Guid.NewGuid();
            var endpoints = Builder<EndpointResponse>.CreateListOfSize(3).Build().ToList();
            var json = JsonConvert.SerializeObject(endpoints, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });

            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When($"{baseAddress}/conferences/{conferenceId}/endpoints")
                .Respond(HttpStatusCode.OK, "application/json", json);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);

            var videoApiClient = VideoApiClient.GetClient(baseAddress, httpClient);
            var videoApiService = new VideoApiService(videoApiClient, new Mock<ILogger<VideoApiService>>().Object);

            // act
            var result = await videoApiService.GetEndpointsForConference(conferenceId);

            // assert
            mockHttp.GetMatchCount(request).Should().Be(1);
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(endpoints);
        }
    }
}