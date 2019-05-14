using System.Collections.Generic;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingIsReadyForVideoIntegrationEvent : IntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
        public override IntegrationEventType EventType => IntegrationEventType.HearingIsReadyForVideo;
    }
}