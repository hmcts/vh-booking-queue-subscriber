using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToUpdateParticipantMapper
    {
        public static UpdateParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            var updatedParticipant = new UpdateParticipantRequest
            {
                Fullname = participant.Fullname,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                Username = participant.Username,
                LinkedParticipants = LinkedParticipantToRequestMapper
                    .MapToLinkedParticipantRequestList(participant.LinkedParticipants)
            };
            return updatedParticipant;
        }
    }
}