using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class AllocationHearingHandlerTests : MessageHandlerTestBase
    {

        [Test]
        public async Task should_call_videoweb_service_when_request_is_valid()
        {
            var messageHandler = new AllocationHearingHandler(VideoWebServiceMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoWebServiceMock.Verify(x=>x.PushAllocationToCsoUpdatedMessage(It.IsAny<AllocationHearingsToCsoRequest>()), Times.Once);
        }
        
        [Test]
        public async Task should_call_videoweb_service_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new AllocationHearingHandler(VideoWebServiceMock.Object);

            var integrationEvent = new AllocationHearingsIntegrationEvent {Hearings = buildHearings(), AllocatedCso = buildCsoUser()};
            await messageHandler.HandleAsync(integrationEvent);
            VideoWebServiceMock.Verify(x=>x.PushAllocationToCsoUpdatedMessage(It.IsAny<AllocationHearingsToCsoRequest>()), Times.Once);
        }
        
        private static AllocationHearingsIntegrationEvent GetIntegrationEvent()
        {
            return new AllocationHearingsIntegrationEvent
            {
                Hearings = buildHearings(),
                AllocatedCso = buildCsoUser()
            };
        }

        private static UserDto buildCsoUser()
        {
            return new UserDto() {Username = "user.name@mail.com"};
        }

        private static IList<HearingAllocationDto> buildHearings()
        {
            IList<HearingAllocationDto> list = new List<HearingAllocationDto>();

            for (int i = 0; i < 3; i++)
            {
                HearingAllocationDto hearing = new HearingAllocationDto()
                {
                    HearingId = Guid.NewGuid(),
                    CaseName = $"Case name {i}",
                    JudgeDisplayName = $"Judge {i}",
                    ScheduledDateTime = DateTime.Now
                };
                list.Add(hearing);
            }
            return list;
        }
    }
}