using VideoApi.Contract.Requests;
using ConferenceRoomType = VideoApi.Contract.Enums.ConferenceRoomType;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class HearingToUpdateConferenceMapper
    {
        public static UpdateConferenceRequest MapToUpdateConferenceRequest(HearingDto hearing)
        {
            return new UpdateConferenceRequest
            {
                HearingRefId = hearing.HearingId,
                CaseName = hearing.CaseName,
                CaseNumber = hearing.CaseNumber,
                CaseType = hearing.CaseType,
                ScheduledDateTime = hearing.ScheduledDateTime,
                ScheduledDuration = hearing.ScheduledDuration,
                HearingVenueName = hearing.HearingVenueName,
                AudioRecordingRequired = hearing.RecordAudio,
                RoomType = (ConferenceRoomType)hearing.ConferenceRoomType
            };
        }
    }
}