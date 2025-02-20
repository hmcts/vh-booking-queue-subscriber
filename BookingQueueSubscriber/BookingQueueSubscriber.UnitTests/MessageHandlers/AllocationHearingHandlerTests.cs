using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class AllocationHearingHandlerTests : MessageHandlerTestBase
    {
        [Test]
        public async Task should_call_videoweb_service_when_request_is_valid()
        {
            var messageHandler = new AllocationHearingHandler(VideoWebServiceMock.Object,VideoApiServiceMock.Object);
            
            var integrationEvent = GetIntegrationEvent();
            await messageHandler.HandleAsync(integrationEvent);
            VideoWebServiceMock.Verify(x=>x.PushAllocationToCsoUpdatedMessage(It.IsAny<HearingAllocationNotificationRequest>()), Times.Once);
        }
        
        [Test]
        public async Task should_call_videoweb_service_when_handle_is_called_with_explicit_interface()
        {
            var messageHandler = (IMessageHandler)new AllocationHearingHandler(VideoWebServiceMock.Object, VideoApiServiceMock.Object);

            var hearings = BuildHearings();
            var conferences = BuilderConferences(hearings);
            VideoApiServiceMock
                .Setup(x => x.GetConferencesByHearingRefIdsAsync(It.IsAny<GetConferencesByHearingIdsRequest>()))
                .ReturnsAsync(conferences);
            var integrationEvent = new HearingsAllocatedIntegrationEvent {Hearings = hearings, AllocatedCso = BuildCsoUser()};
            await messageHandler.HandleAsync(integrationEvent);
            VideoWebServiceMock.Verify(x=>x.PushAllocationToCsoUpdatedMessage(It.IsAny<HearingAllocationNotificationRequest>()), Times.Once);
        }
        
        private HearingsAllocatedIntegrationEvent GetIntegrationEvent()
        {
            var hearings = BuildHearings();
            var conferences = BuilderConferences(hearings);

            VideoApiServiceMock
                .Setup(x => x.GetConferencesByHearingRefIdsAsync(It.IsAny<GetConferencesByHearingIdsRequest>()))
                .ReturnsAsync(conferences);
                
            return new HearingsAllocatedIntegrationEvent
            {
                Hearings = BuildHearings(),
                AllocatedCso = BuildCsoUser()
            };
        }

        private static JusticeUserDto BuildCsoUser()
        {
            return new JusticeUserDto() {Username = "user.name@mail.com", FullName = "CSO Name", UserId = Guid.NewGuid()};
        }

        private static IList<HearingDto> BuildHearings()
        {
            IList<HearingDto> list = new List<HearingDto>();

            for (int i = 0; i < 3; i++)
            {
                HearingDto hearing = new HearingDto()
                {
                    HearingId = Guid.NewGuid(),
                    CaseName = $"Case name {i}",
                    ScheduledDateTime = DateTime.Now
                };
                list.Add(hearing);
            }
            return list;
        }

        private static IList<ConferenceDetailsResponse> BuilderConferences(IList<HearingDto> hearings)
        {
            return hearings.Select(h => new ConferenceDetailsResponse()
            {
                Id = Guid.NewGuid(),
                HearingId = h.HearingId,
                CaseName = h.CaseName,
                ScheduledDateTime = h.ScheduledDateTime
            }).ToList();
        }
    }
}