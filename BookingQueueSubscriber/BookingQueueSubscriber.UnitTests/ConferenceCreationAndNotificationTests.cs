using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using BookingsApi.Contract.Requests;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UserApi.Client;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
namespace BookingQueueSubscriber.UnitTests
{
    public class ConferenceCreationAndNotificationTests
    {
        private Mock<INotificationService> _notificationServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IBookingsApiClient> _bookingsAPIMock;
        private Mock<ILogger<UserCreationAndNotification>> _logger;
        private Mock<ILogger<UserService>> _logger2;
        private Mock<IUserApiClient> _userApi;
        private Mock<IVideoApiService> _videoApiServiceMock;
        private Mock<IUserCreationAndNotification> _userCreationAndNotification;
        private Mock<IVideoWebService> _videoWebServiceMock;
        
        public ConferenceCreationAndNotificationTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _userServiceMock = new Mock<IUserService>();
            _bookingsAPIMock = new Mock<IBookingsApiClient>();
            _userApi = new Mock<IUserApiClient>();
            _videoApiServiceMock = new Mock<IVideoApiService>();
            _userCreationAndNotification = new Mock<IUserCreationAndNotification>();
            _videoWebServiceMock = new Mock<IVideoWebService>();
        }
        
        [Test]
        public async Task should_call_video_and_bookings_api_when_request_is_valid()
        {
            var expectedConferenceId = Guid.NewGuid();
            var conferenceCreationAndNotification = new ConferenceCreationAndNotification(_userCreationAndNotification.Object,
                _videoApiServiceMock.Object,
                _bookingsAPIMock.Object,
                _videoWebServiceMock.Object);
            _videoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse{ Id = expectedConferenceId});

            var request = CreateRequest();
            await conferenceCreationAndNotification.CreateConferenceAndNotifyAsync(request);
            
            _videoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            _bookingsAPIMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()), Times.Once);
            _videoWebServiceMock.Verify(x => x.PushNewConferenceAdded(expectedConferenceId));
        }

        private static CreateConferenceAndNotifyRequest CreateRequest()
        {
            var hearingDto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Generic",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true
            };
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build().ToList();

            var endpoints = Builder<EndpointDto>.CreateListOfSize(4).Build().ToList();

            return new CreateConferenceAndNotifyRequest
            {
                Hearing = hearingDto,
                ParticipantUsersToCreate = participants,
                Participants = participants,
                Endpoints = endpoints
            };
        }
    }
}
