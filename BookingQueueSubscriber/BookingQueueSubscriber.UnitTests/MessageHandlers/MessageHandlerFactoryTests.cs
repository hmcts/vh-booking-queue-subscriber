using System;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class MessageHandlerFactoryTests : MessageHandlerTestBase
    {
        [TestCase(MessageType.HearingIsReadyForVideo, typeof(HearingReadyForVideoHandler))]
        [TestCase(MessageType.ParticipantAdded, typeof(ParticipantAddedHandler))]
        [TestCase(MessageType.ParticipantRemoved, typeof(ParticipantRemovedHandler))]
        [TestCase(MessageType.HearingDetailsUpdated, typeof(HearingDetailsUpdatedHandler))]
        [TestCase(MessageType.HearingCancelled, typeof(HearingCancelledHandler))]
        public void should_return_instance_of_message_handler_for_given_message_type(MessageType messageType,
            Type messageHandlerType)
        {
            var messageHandlerFactory = new MessageHandlerFactory(MessageHandlersList);


            var handler = messageHandlerFactory.Get(messageType);
            handler.Should().BeOfType(messageHandlerType);
        }
    }
}