using BookingQueueSubscriber.Services.MessageHandlers.Extensions;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToParticipantRequestMapper
    {
        public static ParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            var request = new ParticipantRequest
            {
                Id = participant.ParticipantId,
                Username = participant.Username,
                ContactEmail = participant.ContactEmail,
                DisplayName = participant.DisplayName,
                UserRole = participant.MapUserRoleToContractEnum(),
                HearingRole = participant.HearingRole,
                ParticipantRefId = participant.ParticipantId,
                LinkedParticipants = LinkedParticipantToRequestMapper.MapToLinkedParticipantRequestList(participant.LinkedParticipants)
            };
            
            return request;
        }

        public static ParticipantRequest MapToParticipantRequest(ParticipantDto participant, Guid participantId, Guid participantRefId)
        {
            var request = MapToParticipantRequest(participant);
            request.Id = participantId;
            request.ParticipantRefId = participantRefId;
            return request;
        }
    }
}