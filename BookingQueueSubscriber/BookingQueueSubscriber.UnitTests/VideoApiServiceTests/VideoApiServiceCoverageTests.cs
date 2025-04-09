using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;
using VideoApi.Client;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.VideoApiServiceTests;

[TestFixture]
public class VideoApiServiceCoverageTests
{
    private Mock<IVideoApiClient> _apiClientMock;
    private Mock<ILogger<VideoApiService>> _loggerMock;
    private VideoApiService _videoApiService;

    [SetUp]
    public void Setup()
    {
        _apiClientMock = new Mock<IVideoApiClient>();
        _loggerMock = new Mock<ILogger<VideoApiService>>();
        _videoApiService = new VideoApiService(_apiClientMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task BookNewConferenceAsync_Should_Call_ApiClient()
    {
        var request = new BookNewConferenceRequest();
        await _videoApiService.BookNewConferenceAsync(request);
        _apiClientMock.Verify(x => x.BookNewConferenceAsync(request), Times.Once);
    }

    [Test]
    public async Task UpdateConferenceAsync_Should_Call_ApiClient()
    {
        var request = new UpdateConferenceRequest();
        await _videoApiService.UpdateConferenceAsync(request);
        _apiClientMock.Verify(x => x.UpdateConferenceAsync(request), Times.Once);
    }

    [Test]
    public async Task DeleteConferenceAsync_Should_Call_ApiClient()
    {
        var conferenceId = Guid.NewGuid();
        await _videoApiService.DeleteConferenceAsync(conferenceId);
        _apiClientMock.Verify(x => x.RemoveConferenceAsync(conferenceId), Times.Once);
    }

    [Test]
    public async Task GetEndpointsForConference_Should_Call_ApiClient()
    {
        var conferenceId = Guid.NewGuid();
        await _videoApiService.GetEndpointsForConference(conferenceId);
        _apiClientMock.Verify(x => x.GetEndpointsForConferenceAsync(conferenceId), Times.Once);
    }

    [Test]
    public async Task AddParticipantsToConference_Should_Call_ApiClient()
    {
        var conferenceId = Guid.NewGuid();
        var request = new AddParticipantsToConferenceRequest();
        await _videoApiService.AddParticipantsToConference(conferenceId, request);
        _apiClientMock.Verify(x => x.AddParticipantsToConferenceAsync(conferenceId, request), Times.Once);
    }

    [Test]
    public async Task RemoveParticipantFromConference_Should_Call_ApiClient()
    {
        var conferenceId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        await _videoApiService.RemoveParticipantFromConference(conferenceId, participantId);
        _apiClientMock.Verify(x => x.RemoveParticipantFromConferenceAsync(conferenceId, participantId), Times.Once);
    }
}