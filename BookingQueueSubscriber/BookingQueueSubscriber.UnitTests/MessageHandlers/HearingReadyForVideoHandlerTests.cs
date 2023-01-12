using System;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using BookingsApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingReadyForVideoHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_video_and_bookings_api_when_request_is_valid()
        {
            var messageHandler = new HearingReadyForVideoHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object,
                UserCreationAndNotificationMock.Object, BookingsApiClientMock.Object );
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());
         
            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()), Times.Once);
        }

        [Test]
        public async Task should_call_video_and_bookings_api_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, UserCreationAndNotificationMock.Object, BookingsApiClientMock.Object);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse());

            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
            BookingsApiClientMock.Verify(x => x.UpdateBookingStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateBookingStatusRequest>()), Times.Once);
        }

        [Test]
        public async Task Pushes_New_Conference_Added_Event_To_Video_Web()
        {
            var expectedConferenceId = Guid.NewGuid();
            var messageHandler = (IMessageHandler) new HearingReadyForVideoHandler(VideoApiServiceMock.Object,
                VideoWebServiceMock.Object, UserCreationAndNotificationMock.Object, BookingsApiClientMock.Object);
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>())).ReturnsAsync(new ConferenceDetailsResponse { Id = expectedConferenceId});

            var integrationEvent = CreateEvent();
            await messageHandler.HandleAsync(integrationEvent);

            VideoWebServiceMock.Verify(x => x.PushNewConferenceAdded(expectedConferenceId));
        }


        private static HearingIsReadyForVideoIntegrationEvent CreateEvent()
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