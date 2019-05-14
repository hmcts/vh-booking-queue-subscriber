using System;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class ParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string HearingRole { get; set; }
        public string UserRole { get; set; }
        public CaseRoleGroup CaseGroupType { get; set; }
        public string Representee { get; set; }
    }

    public enum CaseRoleGroup
    {
        PartyGroup0 = 0,
        PartyGroup1 = 1,
        PartyGroup2 = 2,
        PartyGroup3 = 3,
    }
}