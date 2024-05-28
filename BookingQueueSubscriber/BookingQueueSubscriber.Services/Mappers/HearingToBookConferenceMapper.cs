using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.Mappers;

public static class HearingToBookConferenceMapper
{
    public static BookNewConferenceRequest MapToBookNewConferenceRequest(HearingDto hearingDto,
        IEnumerable<ParticipantDto> participantDtos,
        IEnumerable<EndpointDto> endpointDtos)
    {
        var participants = participantDtos
            .Select(ParticipantToParticipantRequestMapper.MapToParticipantRequest)
            .ToList();

        var request = new BookNewConferenceRequest
        {
            CaseNumber = hearingDto.CaseNumber,
            CaseName = hearingDto.CaseName,
            CaseType = hearingDto.CaseType,
            ScheduledDuration = hearingDto.ScheduledDuration,
            ScheduledDateTime = hearingDto.ScheduledDateTime,
            HearingRefId = hearingDto.HearingId,
            Participants = participants,
            HearingVenueName = hearingDto.HearingVenueName,
            AudioRecordingRequired = hearingDto.RecordAudio,
            Endpoints = endpointDtos?.Select(EndpointToRequestMapper.MapToRequest).ToList(),
            CaseTypeServiceId = hearingDto.CaseTypeServiceId
        };

        return request;
    }
}