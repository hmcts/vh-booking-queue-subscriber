using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using NotificationApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public abstract class MessageHandlerTestBase
    {
        protected Mock<IVideoApiService> VideoApiServiceMock { get; set; }
        protected Mock<IVideoWebService> VideoWebServiceMock { get; set; }
        protected Mock<IUserService> UserServiceMock { get; set; }
        protected Mock<INotificationService> NotificationServiceMock { get; set; }
        protected Mock<INotificationApiClient> NotificationApiClientMock { get; set; }
        protected Mock<IBookingsApiClient> BookingsApiClientMock { get; set; }
        protected Guid ParticipantId { get; set; }
        protected Guid HearingId { get; set; }
        protected ConferenceDetailsResponse ConferenceDetailsResponse { get; set; }

        [SetUp]
        public void Setup()
        {
            ParticipantId = Guid.NewGuid();
            HearingId = Guid.NewGuid();
            VideoApiServiceMock = new Mock<IVideoApiService>();
            ConferenceDetailsResponse = new ConferenceDetailsResponse
            {
                Id = Guid.NewGuid(),
                Participants = new List<ParticipantResponse>
                {
                    new()
                    {
                        Id = Guid.NewGuid(), 
                        RefId = ParticipantId, 
                        ContactEmail = "test@hmcts.net", 
                        Username = "test@hmcts.net"
                    }
                }
            };

            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(ConferenceDetailsResponse);

            VideoWebServiceMock = new Mock<IVideoWebService>();
            UserServiceMock = new Mock<IUserService>();
            NotificationServiceMock = new Mock<INotificationService>();
            BookingsApiClientMock = new Mock<IBookingsApiClient>();
            NotificationApiClientMock = new Mock<INotificationApiClient>();
        }
    }
}