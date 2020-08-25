using System;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class StartupTests
    {
        [TestCase(typeof(AzureAdConfiguration), typeof(AzureAdConfiguration))]
        [TestCase(typeof(HearingServicesConfiguration), typeof(HearingServicesConfiguration))]
        [TestCase(typeof(IVideoApiService), typeof(VideoApiServiceFake))]
        [TestCase(typeof(IMessageHandlerFactory), typeof(MessageHandlerFactory))]
        [TestCase(typeof(IMessageHandler<HearingCancelledIntegrationEvent>), typeof(HearingCancelledHandler))]
        [TestCase(typeof(IMessageHandler<HearingDetailsUpdatedIntegrationEvent>), typeof(HearingDetailsUpdatedHandler))]
        [TestCase(typeof(IMessageHandler<HearingIsReadyForVideoIntegrationEvent>), typeof(HearingReadyForVideoHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantRemovedIntegrationEvent>), typeof(ParticipantRemovedHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantsAddedIntegrationEvent>), typeof(ParticipantsAddedHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantUpdatedIntegrationEvent>), typeof(ParticipantUpdatedHandler))]
        public void should_return_services(Type serviceType, Type serviceInstance)
        {
            var service = ServiceProviderFactory.ServiceProvider.GetService(serviceType);
            service.Should().BeOfType(serviceInstance);
        }
    }
}