using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public abstract class MessageHandlerTestBase
    {
        protected Mock<IVideoApiService> VideoApiServiceMock { get; set; }
        protected Mock<IVideoWebService> VideoWebServiceMock { get; set; }
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
                Participants = new List<ParticipantDetailsResponse>
                {
                    new ParticipantDetailsResponse {Id = Guid.NewGuid(), RefId = ParticipantId}
                }
            };

            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(HearingId, It.IsAny<bool>())).ReturnsAsync(ConferenceDetailsResponse);

            VideoWebServiceMock = new Mock<IVideoWebService>();
        }
    }
}