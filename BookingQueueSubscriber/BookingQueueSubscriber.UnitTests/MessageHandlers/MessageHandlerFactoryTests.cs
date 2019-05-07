using System;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class MessageHandlerFactoryTests : MessageHandlerTestBase
    {
        [TestCase(IntegrationEventType.HearingIsReadyForVideo, typeof(HearingReadyForVideoHandler))]
        [TestCase(IntegrationEventType.ParticipantAdded, typeof(ParticipantAddedHandler))]
        [TestCase(IntegrationEventType.ParticipantRemoved, typeof(ParticipantRemovedHandler))]
        [TestCase(IntegrationEventType.HearingDetailsUpdated, typeof(HearingDetailsUpdatedHandler))]
        [TestCase(IntegrationEventType.HearingCancelled, typeof(HearingCancelledHandler))]
        public void should_return_instance_of_message_handler_for_given_message_type(IntegrationEventType integrationEventType,
            Type messageHandlerType)
        {
            var messageHandlerFactory = new MessageHandlerFactory(MessageHandlersList);


            var handler = messageHandlerFactory.Get(integrationEventType);
            handler.Should().BeOfType(messageHandlerType);
        }
    }
}