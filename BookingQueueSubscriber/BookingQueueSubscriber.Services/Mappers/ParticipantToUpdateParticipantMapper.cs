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
                ContactEmail = participant.ContactEmail,
                DisplayName = participant.DisplayName,
                Username = participant.Username,
                LinkedParticipants = LinkedParticipantToRequestMapper
                    .MapToLinkedParticipantRequestList(participant.LinkedParticipants),
                UserRole = participant.MapUserRoleToContractEnum(),
                HearingRole = participant.HearingRole
            };
        }
    }
}