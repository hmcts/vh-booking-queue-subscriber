using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public static partial class VideoApiServiceLogger
{
    [LoggerMessage(EventId= 2300, Level = LogLevel.Information, Message = "Booking new conference for hearing {Hearing}")]
    public static partial void BookingNewConference(this ILogger logger, Guid hearing);

    [LoggerMessage(EventId= 2301, Level = LogLevel.Information, Message = "Updating conference with hearing id {Hearing}")]
    public static partial void UpdatingConference(this ILogger logger, Guid hearing);

    [LoggerMessage(EventId= 2302, Level = LogLevel.Information, Message = "Deleting conference id {ConferenceId}")]
    public static partial void DeletingConference(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId= 2303, Level = LogLevel.Information, Message = "Getting conference by hearing ref id {HearingId}")]
    public static partial void GettingConferenceByHearing(this ILogger logger, Guid hearingId);

    [LoggerMessage(EventId= 2304, Level = LogLevel.Information, Message = "Getting endpoints by conference id {ConferenceId}")]
    public static partial void GettingEndpoints(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId= 2305, Level = LogLevel.Information, Message = "Adding participants to conference {ConferenceId}")]
    public static partial void AddingParticipants(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId= 2306, Level = LogLevel.Information, Message = "Removing participant {ParticipantId} from conference {ConferenceId}")]
    public static partial void RemovingParticipant(this ILogger logger, Guid participantId, Guid conferenceId);

    [LoggerMessage(EventId= 2307, Level = LogLevel.Information, Message = "Updating participants in conference {ConferenceId} with request: {Request}")]
    public static partial void UpdatingParticipants(this ILogger logger, Guid conferenceId, object request);

    [LoggerMessage(EventId= 2308, Level = LogLevel.Information, Message = "Updating participant {ParticipantId} in conference {ConferenceId}")]
    public static partial void UpdatingParticipant(this ILogger logger, Guid participantId, Guid conferenceId);

    [LoggerMessage(EventId= 2309, Level = LogLevel.Information, Message = "Adding endpoint to conference: {ConferenceId}")]
    public static partial void AddingEndpoint(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId= 2310, Level = LogLevel.Information, Message = "Removing endpoint {Sip} from conference {ConferenceId}")]
    public static partial void RemovingEndpoint(this ILogger logger, string sip, Guid conferenceId);

    [LoggerMessage(EventId= 2311, Level = LogLevel.Information, Message = "Updating endpoint {Sip} in conference {ConferenceId}")]
    public static partial void UpdatingEndpoint(this ILogger logger, string sip, Guid conferenceId);

    [LoggerMessage(EventId= 2312, Level = LogLevel.Information, Message = "Closing consultation for conference {ConferenceId}")]
    public static partial void ClosingConsultation(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId= 2313, Level = LogLevel.Information, Message = "Updating username for participant {ParticipantId}")]
    public static partial void UpdatingParticipantUsername(this ILogger logger, Guid participantId);
}
