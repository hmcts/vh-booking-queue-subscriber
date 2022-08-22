﻿using System;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class JudgeUpdatedIntegrationEvent : IIntegrationEvent
    {
        public HearingDto Hearing { get; set; }
        public ParticipantDto Judge { get; set; }
    }
}