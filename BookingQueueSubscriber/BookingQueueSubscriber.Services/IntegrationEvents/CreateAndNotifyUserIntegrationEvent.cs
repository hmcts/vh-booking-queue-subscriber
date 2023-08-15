﻿namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class CreateAndNotifyUserIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public IList<ParticipantDto> Participants { get; set; }
    }
}

    