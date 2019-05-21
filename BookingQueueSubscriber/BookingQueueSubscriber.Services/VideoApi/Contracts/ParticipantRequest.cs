using System;

namespace BookingQueueSubscriber.Services.VideoApi.Contracts
{
    public class ParticipantRequest
    {
        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }
        public string CaseTypeGroup { get; set; }
        public string Representee { get; set; }
    }
    
    public enum UserRole
    {
        None = 0,
        CaseAdmin = 1,
        VideoHearingsOfficer = 2,
        HearingFacilitationSupport = 3,
        Judge = 4,
        Individual = 5,
        Representative = 6
    }
}