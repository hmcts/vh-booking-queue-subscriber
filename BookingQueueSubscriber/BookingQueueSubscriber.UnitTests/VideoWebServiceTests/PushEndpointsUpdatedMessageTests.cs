using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoWeb;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests
{
    public class PushEndpointsUpdatedMessageTests
    {
        [Test]
        public async Task should_call_video_web_with_provided_request_object()
        {
            // arrange
            var conferenceId = Guid.NewGuid();
            var payload = new UpdateConferenceEndpointsRequest
            {
                NewEndpoints = new List<EndpointResponse>()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Pin = "1234",
                        DefenceAdvocate = "advocate@email.com",
                        DisplayName = "newly created endpoint",
                        SipAddress = "133545@something"
                    }
                }
            };
            
            var json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            
            var mockHttp = new MockHttpMessageHandler();
            var request = mockHttp.When($"http://video-web/internalevent/EndpointsUpdated")
                .WithQueryString("conferenceId", conferenceId.ToString())
                .WithContent(json)
                .Respond(HttpStatusCode.NoContent);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://video-web");
            var service = new VideoWebService(httpClient, new Mock<ILogger<VideoWebService>>().Object);

            // act
            await service.PushEndpointsUpdatedMessage(conferenceId, payload);
            
            // assert
            mockHttp.GetMatchCount(request).Should().Be(1);
        }
    }
}