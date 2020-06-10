using System;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class ParticipantToParticipantRequestMapper
    {
        public static ParticipantRequest MapToParticipantRequest(ParticipantDto participantDto)
        {
            var request = new ParticipantRequest
            {
                Name = participantDto.Fullname,
                Username = participantDto.Username,
                FirstName = participantDto.FirstName,
                LastName = participantDto.LastName,
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