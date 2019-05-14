using System.Collections.Generic;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public abstract class MessageHandlerTestBase
    {
        protected List<IMessageHandler> MessageHandlersList { get; set; }
        protected Mock<IVideoApiService> VideoApiServiceMock { get; set; }
        
        [SetUp]
        public void Setup()
        {
            VideoApiServiceMock = new Mock<IVideoApiService>();
            MessageHandlersList = new List<IMessageHandler>
            {
                new HearingReadyForVideoHandler(VideoApiServiceMock.Object),
                new ParticipantAddedHandler(VideoApiServiceMock.Object),
                new ParticipantRemovedHandler(VideoApiServiceMock.Object),
                new HearingDetailsUpdatedHandler(VideoApiServiceMock.Object),
                new HearingCancelledHandler(VideoApiServiceMock.Object)
            };
        }
    }
}