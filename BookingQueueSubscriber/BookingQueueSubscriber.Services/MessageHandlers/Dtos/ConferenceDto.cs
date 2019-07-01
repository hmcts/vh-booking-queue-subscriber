using System;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class ConferenceDto
    {
        public Guid Id { get; set; }
        public Guid HearingRefId { get; set; }
    }
}