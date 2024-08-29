using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class HearingCancelledHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_services_when_request_is_valid()
        {
            var messageHandler = new HearingCancelledHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = new HearingCancelledIntegrationEvent {HearingId = HearingId};
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.DeleteConferenceAsync(It.IsAny<Guid>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingCancelledMessage(HearingId));
        }

        [Test]
        public async Task should_call_services_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new HearingCancelledHandler(VideoApiServiceMock.Object, VideoWebServiceMock.Object);

            var integrationEvent = new HearingCancelledIntegrationEvent { HearingId = HearingId };
            await messageHandler.HandleAsync(integrationEvent);
            VideoApiServiceMock.Verify(x => x.DeleteConferenceAsync(It.IsAny<Guid>()), Times.Once);
            VideoWebServiceMock.Verify(x => x.PushHearingCancelledMessage(HearingId));
        }
    }
}