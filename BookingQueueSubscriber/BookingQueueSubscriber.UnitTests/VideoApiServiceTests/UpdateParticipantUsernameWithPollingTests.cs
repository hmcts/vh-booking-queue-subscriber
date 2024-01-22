using System.Net;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using Microsoft.Extensions.Logging;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.VideoApiServiceTests
{
    public class UpdateParticipantUsernameWithPollingTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IVideoApiClient> _videoApiClientMock;
        private Mock<ILogger<VideoApiService>> _loggerMock;
        private IVideoApiService _videoApiService;
        private Guid _hearingId;
        private Guid _participantId;
        
        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _videoApiClientMock = new Mock<IVideoApiClient>();
            _loggerMock = new Mock<ILogger<VideoApiService>>();
            _videoApiService = new VideoApiService(_videoApiClientMock.Object, _loggerMock.Object);
            _hearingId = Guid.NewGuid();
            _participantId = Guid.NewGuid();
        }
        
        [Test]
        public void should_poll_video_api_for_response_then_throw_error()
        {
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User() { UserName = "username"});
            
            //video mock should throw not found exception
            _videoApiClientMock.Setup(x => x.GetConferenceByHearingRefIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null));
            
            //assert that message handler throws exception
            Assert.ThrowsAsync<VideoApiException>(() => _videoApiService.UpdateParticipantUsernameWithPolling(_hearingId, "username", "email@email.com"));
        }
    
        [Test]
        public async Task should_poll_video_api_for_response_then_return_it_after_initial_error()
        {
            _userServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new User() { UserName = "username"});
            
            //video mock should throw not found exception, then return conference on second iteration
            _videoApiClientMock.SetupSequence(x => x.GetConferenceByHearingRefIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null))
                .ReturnsAsync(new ConferenceDetailsResponse()
                {
                    Participants = new List<ParticipantDetailsResponse>()
                    {
                        new()
                        {
                            Id = _participantId,
                            ContactEmail = "email@email.com"
                        }
                    }
                });

            await _videoApiService.UpdateParticipantUsernameWithPolling(_hearingId, "username", "email@email.com");
            //assert service does not throw exception
            Assert.Pass();
        }
    }
}
