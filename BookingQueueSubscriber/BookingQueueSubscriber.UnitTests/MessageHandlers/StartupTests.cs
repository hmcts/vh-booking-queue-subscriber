using System;
using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers
{
    public class StartupTests
    {
        [TestCase(typeof(IOptionsSnapshot<AzureAdConfiguration>), typeof(OptionsManager<AzureAdConfiguration>))]
        [TestCase(typeof(IOptionsSnapshot<ServicesConfiguration>), typeof(OptionsManager<ServicesConfiguration>))]
        [TestCase(typeof(IVideoApiService), typeof(VideoApiServiceFake))]
        [TestCase(typeof(IMemoryCache), typeof(MemoryCache))]
        [TestCase(typeof(IMessageHandlerFactory), typeof(MessageHandlerFactory))]
        [TestCase(typeof(IMessageHandler<HearingCancelledIntegrationEvent>), typeof(HearingCancelledHandler))]
        [TestCase(typeof(IMessageHandler<HearingDetailsUpdatedIntegrationEvent>), typeof(HearingDetailsUpdatedHandler))]
        [TestCase(typeof(IMessageHandler<HearingIsReadyForVideoIntegrationEvent>), typeof(HearingReadyForVideoHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantRemovedIntegrationEvent>), typeof(ParticipantRemovedHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantsAddedIntegrationEvent>), typeof(ParticipantsAddedHandler))]
        [TestCase(typeof(IMessageHandler<ParticipantUpdatedIntegrationEvent>), typeof(ParticipantUpdatedHandler))]
        [TestCase(typeof(IMessageHandler<HearingsAllocationIntegrationEvent>), typeof(HearingsAllocationHandler))]
        public void should_return_services(Type serviceType, Type serviceInstance)
        {
            var service = ServiceProviderFactory.ServiceProvider.GetService(serviceType);
            service.Should().BeOfType(serviceInstance);
        }
    }
}