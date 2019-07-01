using System;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class ParticipantAddedIntegrationEvent: IIntegrationEvent
    {

        public Guid HearingId { get; }
        public ParticipantDto Participant { get; }
    }
}