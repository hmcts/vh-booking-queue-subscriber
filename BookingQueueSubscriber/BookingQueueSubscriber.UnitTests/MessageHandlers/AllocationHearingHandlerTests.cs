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
        private readonly Mock<ILogger<AllocationHearingHandler>> _logger =
            new Mock<ILogger<AllocationHearingHandler>>();

        [Test]
        public async Task should_call_videoweb_service_when_request_is_valid()
        {
            var messageHandler = new AllocationHearingHandler(VideoWebServiceMock.Object, _logger.Object, BookingsApiClientMock.Object);

            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoWebServiceMock.Verify(x=>x.PushAllocationToCsoUpdatedMessage(It.IsAny<AllocationHearingsToCsoRequest>()), Times.Once);
        }
        private AllocationHearingsIntegrationEvent GetIntegrationEvent()
        {
            return new AllocationHearingsIntegrationEvent
            {
                Hearings = new List<HearingDto>(),
                AllocatedCso = new UserDto()
            };
        }


    }
}