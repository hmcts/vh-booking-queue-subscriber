using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
                ContactEmail = participantDto.ContactEmail,
                ContactTelephone = participantDto.ContactTelephone,
                DisplayName = participantDto.DisplayName,
                UserRole = GetUserRole(participantDto.UserRole),
                HearingRole = participantDto.HearingRole,
                CaseTypeGroup = participantDto.CaseGroupType.ToString(),
                ParticipantRefId = participantDto.ParticipantId,
                Representee = participantDto.Representee
            };
            if (participantDto.LinkedParticipants != null && participantDto.LinkedParticipants.Any())
            {
                request.LinkedParticipants = participantDto?.LinkedParticipants.Select(MapToParticipantRequest);
            }
            return request;
        }

        private static UserRole GetUserRole(string dtoUserRole)
        {
            if (dtoUserRole == "Judicial Office Holder")
            {
                return UserRole.JudicialOfficeHolder;
            }

            else
            {
                return Enum.Parse<UserRole>(dtoUserRole);
            }
        }
    }
}