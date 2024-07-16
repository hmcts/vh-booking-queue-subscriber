using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using BookingsApi.Contract.V1.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingReadyForVideoHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_and_bookings_api_when_request_is_valid()
        {
            var messageHandler = new HearingReadyForVideoHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object,
                 BookingsApiClientMock.Object);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());
         
            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_and_bookings_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, BookingsApiClientMock.Object);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());

            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>()), Times.Once);
        }
        
        [Test]
        public async Task should_call_send_hearing_notification_without_participant_already_notified()
        {
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, BookingsApiClientMock.Object);
            var integrationEvent = CreateEvent(notGeneric: true);
            var usersNotified = new List<UserDto>()
                {new UserDto() {Username = integrationEvent.Participants[0].Username}};
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());
            UserCreationAndNotificationMock.Setup(x => x.SendHearingNotificationAsync(integrationEvent.Hearing, 
                integrationEvent.Participants.Where(dto => usersNotified.All(y=>y.Username != dto.Username))
            ));
            UserCreationAndNotificationMock.Setup(x => 
                    x.CreateUserAndNotifcationAsync(integrationEvent.Hearing, It.IsAny<IList<ParticipantDto>>()))
                .ReturnsAsync(new Func<IList<UserDto>>(() => usersNotified));
            
            
            
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>()), Times.Once);
        }
        
        [Test]
        public async Task should_call_send_hearing_notification_without_create_participant_for_multiday_hearing()
        {
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, BookingsApiClientMock.Object);
            var integrationEvent = CreateEvent(true);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());
            
            await messageHandler.HandleAsync(integrationEvent);
            UserCreationAndNotificationMock.Verify(x => x.SendHearingNotificationAsync(It.IsAny<HearingDto>(), It.IsAny<IEnumerable<ParticipantDto>>()), Times.Never);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Pushes_New_Conference_Added_Event_To_Video_Web()
        {
            var expectedConferenceId = Guid.NewGuid();
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, BookingsApiClientMock.Object);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse { Id = expectedConferenceId});

            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoWebServiceMock.Verify(x => x.PushNewConferenceAdded(expectedConferenceId));
        }

        private static HearingIsReadyForVideoIntegrationEvent CreateEvent(bool isMultiHearing = false, bool notGeneric = false)
        {
            var hearingDto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = (notGeneric) ? "Not-Generic": "Generic",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true,
                GroupId = (isMultiHearing) ? Guid.NewGuid() : null
            };
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build().ToList();

            var endpoints = Builder<EndpointDto>.CreateListOfSize(4).Build().ToList();
            
            var message = new HearingIsReadyForVideoIntegrationEvent
            {
                Hearing = hearingDto,
                Participants = participants,
                Endpoints = endpoints
            };
            return message;
        }
    }
}