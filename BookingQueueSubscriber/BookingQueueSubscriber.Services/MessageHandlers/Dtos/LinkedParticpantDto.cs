using System;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class LinkedParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public Guid LinkedId { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
    
    public enum LinkedParticipantType
    {
        Interpreter = 1
    }
}