using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingDetailsUpdatedHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_services_when_request_is_valid()
        {
            var messageHandler = new HearingDetailsUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateConferenceAsync(It.IsAny<UpdateConferenceRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingDetailsUpdatedMessage(integrationEvent.Hearing.HearingId));
        }

        [Test]
        public async Task should_call_services_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new HearingDetailsUpdatedHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.UpdateConferenceAsync(It.IsAny<UpdateConferenceRequest>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingDetailsUpdatedMessage(integrationEvent.Hearing.HearingId));
        }

        private static HearingDetailsUpdatedIntegrationEvent GetIntegrationEvent()
        {
            return new HearingDetailsUpdatedIntegrationEvent()
            {
                Hearing = new HearingDto
                {
                    CaseName = "CaseName",
                    CaseNumber = "caseNO",
                    CaseType = "caseType",
                    HearingId = Guid.NewGuid(),
                    ScheduledDateTime = DateTime.Now,
                    ScheduledDuration = 30,
                    HearingVenueName = "MyVenue"
                }
            };
        }
    }
}