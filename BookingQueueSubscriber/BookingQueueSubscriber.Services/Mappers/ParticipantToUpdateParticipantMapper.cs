using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToUpdateParticipantMapper
    {
        public static UpdateParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            return new UpdateParticipantRequest
            {
                ParticipantRefId = participant.ParticipantId,
                Fullname = participant.Fullname,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                Username = participant.Username,
                LinkedParticipants = LinkedParticipantToRequestMapper
                    .MapToLinkedParticipantRequestList(participant.LinkedParticipants),
                UserRole = GetUserRole(participant.UserRole),
                HearingRole = participant.HearingRole,
                CaseTypeGroup = participant.CaseGroupType.ToString()
            };
        }
        
        // TODO refactor as duplicated in ParticipantToParticipantRequestMapper
        private static UserRole GetUserRole(string dtoUserRole)
        {
            if (dtoUserRole == UserRoleName.JudicialOfficeHolder)
            {
                return UserRole.JudicialOfficeHolder;
            }
            else if (dtoUserRole == UserRoleName.StaffMember)
            {
                return UserRole.StaffMember;
            }
            else
            {
                return Enum.Parse<UserRole>(dtoUserRole);
            }
        }
    }
}