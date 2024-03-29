using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class MessageHandlerFactoryTests : MessageHandlerTestBase
    {
        public class TestData
        {
            public TestData(IIntegrationEvent integrationEvent, Type messageHandlerType)
            {
                IntegrationEvent = integrationEvent;
                MessageHandlerType = messageHandlerType;
            }

            public IIntegrationEvent IntegrationEvent { get; }
            public Type MessageHandlerType { get; }
        }

        [TestCaseSource(nameof(GetEvents))]
        public void should_return_instance_of_message_handler_for_given_message_type(TestData data)
        {
            var messageHandlerFactory = (IMessageHandlerFactory)ServiceProviderFactory.ServiceProvider.GetService(typeof(IMessageHandlerFactory));

            var handler = messageHandlerFactory.Get(data.IntegrationEvent);
            handler.Should().BeOfType(data.MessageHandlerType);
        }

        [Test]
        public void should_load_handler_for_message()
        {
            var messageHandlerFactory = (IMessageHandlerFactory)ServiceProviderFactory.ServiceProvider.GetService(typeof(IMessageHandlerFactory));
            var integrationEvent = new HearingIsReadyForVideoIntegrationEvent();
            var handler = messageHandlerFactory.Get(integrationEvent);
            handler.Should().BeOfType<HearingReadyForVideoHandler>();
        }

        private static IEnumerable<TestData> GetEvents()
        {
            yield return new TestData(new HearingDetailsUpdatedIntegrationEvent(),typeof(HearingDetailsUpdatedHandler));
            yield return new TestData(new HearingCancelledIntegrationEvent(), typeof(HearingCancelledHandler));
            yield return new TestData(new HearingIsReadyForVideoIntegrationEvent(), typeof(HearingReadyForVideoHandler));
            yield return new TestData(new ParticipantsAddedIntegrationEvent(), typeof(ParticipantsAddedHandler));
            yield return new TestData(new ParticipantRemovedIntegrationEvent(), typeof(ParticipantRemovedHandler));
        }
    }
}