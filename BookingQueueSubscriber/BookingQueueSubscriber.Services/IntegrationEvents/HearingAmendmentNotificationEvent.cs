using System;

namespace BookingQueueSubscriber.Services.IntegrationEvents
{
    public class HearingAmendmentNotificationEvent : IIntegrationEvent
    {
        public HearingConfirmationForParticipantDto HearingConfirmationForParticipant { get; set; }
        public DateTime NewScheduledDateTime { get; set; }
    }
}
