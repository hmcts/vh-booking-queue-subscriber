using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Requests;
using LinkedParticipantType = VideoApi.Contract.Enums.LinkedParticipantType;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class LinkedParticipantToRequestMapper
    {
        public static LinkedParticipantRequest MapToLinkedParticipantRequest(LinkedParticipantDto linkedParticipantDto)
        {
            return new LinkedParticipantRequest
            {
                ParticipantRefId = linkedParticipantDto.ParticipantId,
                LinkedRefId = linkedParticipantDto.LinkedId,
                Type = Enum.Parse<LinkedParticipantType>(linkedParticipantDto.Type.ToString())
            };
        }
        
        public static List<LinkedParticipantRequest> MapToLinkedParticipantRequestList(IList<LinkedParticipantDto> linkedParticipantDtoList)
        {
            if (linkedParticipantDtoList != null && linkedParticipantDtoList.Any())
            {
                return linkedParticipantDtoList.Select(MapToLinkedParticipantRequest).ToList();
            }
            return new List<LinkedParticipantRequest>();
        }
    }
}