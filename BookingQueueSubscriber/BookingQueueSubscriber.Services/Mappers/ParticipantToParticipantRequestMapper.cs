using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoWebRequests = BookingQueueSubscriber.Services.VideoWeb;
using VideoApiRequests = VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToParticipantRequestMapper
    {
        public static VideoApiRequests.ParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            var request = new ParticipantRequest
            {
                Id = participant.ParticipantId,
                Name = participant.Fullname,
                Username = participant.Username,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                DisplayName = participant.DisplayName,
                UserRole = GetUserRole(participant.UserRole),
                HearingRole = participant.HearingRole,
                CaseTypeGroup = participant.CaseGroupType.ToString(),
                ParticipantRefId = participant.ParticipantId,
                Representee = participant.Representee,
                LinkedParticipants = LinkedParticipantToRequestMapper
                    .MapToLinkedParticipantRequestList(participant.LinkedParticipants)
            };
            
            return request;
        }

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