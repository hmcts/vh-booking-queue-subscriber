using BookingQueueSubscriber.Services.MessageHandlers.Extensions;
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
                UserRole = participant.MapUserRoleToContractEnum(),
                HearingRole = participant.HearingRole,
                CaseTypeGroup = "Obsolete"
            };
        }
    }
}