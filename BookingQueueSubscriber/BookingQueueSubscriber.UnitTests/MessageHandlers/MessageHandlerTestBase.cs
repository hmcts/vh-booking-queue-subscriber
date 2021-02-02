using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoApi;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public abstract class MessageHandlerTestBase
    {
        protected Mock<IVideoApiService> VideoApiServiceMock { get; set; }
        protected Guid ParticipantId { get; set; }
        protected Guid HearingId { get; set; }


        [SetUp]
        public void Setup()
        {
            ParticipantId = Guid.NewGuid();
            HearingId = Guid.NewGuid();
            VideoApiServiceMock = new Mock<IVideoApiService>();
            var result = Task.FromResult(new ConferenceResponse
            {
                HearingRefId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Participants = new List<ParticipantResponse>
                {
                    new ParticipantResponse {Id = HearingId, ParticipantRefIid = ParticipantId}
                }
            });

            VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(HearingId)).Returns(result);
        }
    }
}