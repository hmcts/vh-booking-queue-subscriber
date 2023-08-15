using System.Net;
using System.Net.Http;
using System.Threading;
using BookingQueueSubscriber.Services.VideoWeb;
using Microsoft.Extensions.Logging;
using Moq.Protected;

namespace BookingQueueSubscriber.UnitTests.VideoWebServiceTests;

public class EndpointRepPushMethodTests
{
    private VideoWebService _videoWebService;
    private Mock<HttpMessageHandler>  _handler;
    private string _username = "random.user@hmcts.net";
    private string _endpoint = "JVS Endpoint Name";
    private Guid _conferenceId = Guid.NewGuid();

    [SetUp]
    public void Setup()
    {
        _handler = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };
        _handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        var client = new HttpClient(_handler.Object);
        client.BaseAddress = new Uri("http://video-web");
        var logger = new Mock<ILogger<VideoWebService>>();
        _videoWebService = new VideoWebService(client, logger.Object);
    }

    [Test]
    public async Task PushUnlinkedParticipantFromEndpoint()
    {
        await _videoWebService.PushUnlinkedParticipantFromEndpoint(_conferenceId, _username, _endpoint);
        AssertUriCalled($"http://video-web/internalevent/UnlinkedParticipantFromEndpoint?conferenceId={_conferenceId}&participant=random.user@hmcts.net&endpoint=JVS");
    }

    [Test]
    public async Task PushLinkedNewParticipantToEndpoint()
    {
        await _videoWebService.PushLinkedNewParticipantToEndpoint(_conferenceId, _username, _endpoint);
        AssertUriCalled($"http://video-web/internalevent/LinkedNewParticipantToEndpoint?conferenceId={_conferenceId}&participant=random.user@hmcts.net&endpoint=JVS");
    }

    
    [Test]
    public async Task PushCloseConsultationBetweenEndpointAndParticipant() 
    {
        await _videoWebService.PushCloseConsultationBetweenEndpointAndParticipant(_conferenceId, _username, _endpoint);
        AssertUriCalled($"http://video-web/internalevent/CloseConsultationBetweenEndpointAndParticipant?conferenceId={_conferenceId}&participant=random.user@hmcts.net&endpoint=JVS");
    }
    
    private void AssertUriCalled(string requestUri)
    {
        _handler.Protected().Verify("SendAsync", Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(
                request => request.Method == HttpMethod.Post && request.RequestUri.ToString().Contains(requestUri)), ItExpr.IsAny<CancellationToken>());
    }
}
