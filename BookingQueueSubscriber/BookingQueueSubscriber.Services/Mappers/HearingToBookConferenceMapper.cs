using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public class HearingToBookConferenceMapper
    {
        public BookNewConferenceRequest MapToBookNewConferenceRequest(HearingDto hearingDto)
        {
            var participantMapper = new ParticipantToParticipantRequestMapper();
            var participants = hearingDto.Participants.Select(participantMapper.MapToParticipantRequest).ToList();

            var request = new BookNewConferenceRequest
            {
                CaseNumber = hearingDto.CaseNumber,
                CaseName = hearingDto.CaseName,
                CaseType = hearingDto.CaseType,
                ScheduledDuration = hearingDto.ScheduledDuration,
                ScheduledDateTime = hearingDto.ScheduledDateTime,
                HearingRefId = hearingDto.HearingId,
                Participants = participants
            };
            return request;
        }
    }
}