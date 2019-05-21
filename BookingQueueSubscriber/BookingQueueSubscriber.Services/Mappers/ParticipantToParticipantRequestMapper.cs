using System;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public class ParticipantToParticipantRequestMapper
    {
        public ParticipantRequest MapToParticipantRequest(ParticipantDto participantDto)
        {
            var request = new ParticipantRequest
            {
                Name = participantDto.Fullname,
                Username = participantDto.Username,
                DisplayName = participantDto.DisplayName,
                UserRole = Enum.Parse<UserRole>(participantDto.UserRole),
                CaseTypeGroup = participantDto.CaseGroupType.ToString(),
                ParticipantRefId = participantDto.ParticipantId,
                Representee = participantDto.Representee
            };

            return request;
        }
    }
}