using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public static partial class HandlersLogger
{
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Error,
        Message = "Unable to find conference by hearing id {HearingId}")]
    public static partial void ConferenceNotFoundByHearingId(this ILogger logger, Guid hearingId);
    
    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Error,
        Message = "Error notifying participant linked to endpoint")]
    public static partial void ErrorNotifyingLinkedParticipant(this ILogger logger, Exception exception);
    
    [LoggerMessage(
        3003, 
        LogLevel.Error, 
        "Unable to find judge participant by ref id {ParticipantRefId} in {ConferenceId}")]
    public static partial void JudgeParticipantNotFound(this ILogger logger, Guid participantRefId, Guid conferenceId);
    
    [LoggerMessage(
        3004,
        LogLevel.Information, 
        "Update participant list for Conference {ConferenceId}")]
    public static partial void UpdatingParticipantList(this ILogger logger, Guid conferenceId);
    
    [LoggerMessage(
        3005, 
        LogLevel.Error, 
        "Unable to find participant by ref id {ParticipantRefId} in {ConferenceId}")]
    public static partial void ParticipantNotFound(this ILogger logger, Guid participantRefId, Guid conferenceId);

}