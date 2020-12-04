using System;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class ParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string DisplayName { get; set; }
        public string HearingRole { get; set; }
        public string UserRole { get; set; }
        public CaseRoleGroup CaseGroupType { get; set; }
        public string Representee { get; set; }
    }

    public enum CaseRoleGroup
    {
        Judge = 0,
        Claimant = 1,
        Defendant = 2,
        Applicant = 3,
        Respondent = 4,
        Observer = 5,
        PanelMember = 6,
        Appellant = 7,
        State = 8,
        None = 9,
        HomeOffice = 10
    }
}