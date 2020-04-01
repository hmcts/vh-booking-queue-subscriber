using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToUpdateParticipantMapper
    {
        public static UpdateParticipantRequest MapToParticipantRequest(ParticipantDto participant)
        {
            return new UpdateParticipantRequest
            {
                Fullname = participant.Fullname,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee
            };
        }
    }
}