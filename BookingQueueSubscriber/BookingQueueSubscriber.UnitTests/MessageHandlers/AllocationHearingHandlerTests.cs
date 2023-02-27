using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using Moq;
using NUnit.Framework;
using BookingsApi.Contract.Enums;

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

        private static IList<HearingDto> buildHearings()
        {
            IList<HearingDto> list = new List<HearingDto>();

            for (int i = 0; i < 3; i++)
            {
                HearingDto hearing = new HearingDto()
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