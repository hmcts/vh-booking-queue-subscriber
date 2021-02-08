using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToUpdateParticipantMapper
    {
        public static UpdateParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            var request = new UpdateParticipantRequest
            {
                Fullname = participant.Fullname,
                FirstName = participant.FirstName,
                LastName = participant.LastName,
                ContactEmail = participant.ContactEmail,
                ContactTelephone = participant.ContactTelephone,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                Username = participant.Username
            };
            if (participant.LinkedParticipants != null && participant.LinkedParticipants.Any())
            {
                request.LinkedParticipants = participant?.LinkedParticipants.Select(MapToParticipantRequest);
            } 
            return request;
        }
    }
}