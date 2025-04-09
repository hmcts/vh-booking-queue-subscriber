using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public static partial class VideoWebServiceLogger
{
    [LoggerMessage(EventId = 2100, Level = LogLevel.Debug, Message = "Pushing new conference added event for ConferenceId: {ConferenceId}")]
    public static partial void PushNewConferenceAdded(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId = 2101, Level = LogLevel.Debug, Message = "Pushing participants updated message for ConferenceId: {ConferenceId}")]
    public static partial void PushParticipantsUpdated(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId = 2102, Level = LogLevel.Debug, Message = "Participants updated JSON payload: {Payload}, URL: {Url}")]
    public static partial void ParticipantsUpdatedJson(this ILogger logger, string payload, string url);

    [LoggerMessage(EventId = 2103, Level = LogLevel.Debug, Message = "Participants updated message response: {Response}")]
    public static partial void ParticipantsUpdatedResponse(this ILogger logger, HttpResponseMessage response);

    [LoggerMessage(EventId = 2104, Level = LogLevel.Debug, Message = "Pushing allocation to CSO updated message")]
    public static partial void PushAllocationToCso(this ILogger logger);

    [LoggerMessage(EventId = 2105, Level = LogLevel.Debug, Message = "Allocation message response: {Response}")]
    public static partial void AllocationResponse(this ILogger logger, HttpResponseMessage response);

    [LoggerMessage(EventId = 2106, Level = LogLevel.Debug, Message = "Pushing unlinked participant. ConferenceId: {ConferenceId}, Participant: {Participant}, Endpoint: {Endpoint}")]
    public static partial void PushUnlinkedParticipant(this ILogger logger, Guid conferenceId, string participant, string endpoint);

    [LoggerMessage(EventId = 2107, Level = LogLevel.Debug, Message = "Pushing linked new participant. ConferenceId: {ConferenceId}, Participant: {Participant}, Endpoint: {Endpoint}")]
    public static partial void PushLinkedParticipant(this ILogger logger, Guid conferenceId, string participant, string endpoint);

    [LoggerMessage(EventId = 2108, Level = LogLevel.Debug, Message = "Pushing close consultation. ConferenceId: {ConferenceId}, Participant: {Participant}, Endpoint: {Endpoint}")]
    public static partial void PushCloseConsultation(this ILogger logger, Guid conferenceId, string participant, string endpoint);

    [LoggerMessage(EventId = 2109, Level = LogLevel.Debug, Message = "Pushing endpoints updated message for ConferenceId: {ConferenceId}")]
    public static partial void PushEndpointsUpdated(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId = 2110, Level = LogLevel.Debug, Message = "Endpoints updated message response: {Response}")]
    public static partial void EndpointsUpdatedResponse(this ILogger logger, HttpResponseMessage response);

    [LoggerMessage(EventId = 2111, Level = LogLevel.Debug, Message = "Pushing hearing cancelled message for ConferenceId: {ConferenceId}")]
    public static partial void PushHearingCancelled(this ILogger logger, Guid conferenceId);

    [LoggerMessage(EventId = 2112, Level = LogLevel.Debug, Message = "Pushing hearing details updated message for ConferenceId: {ConferenceId}")]
    public static partial void PushHearingDetailsUpdated(this ILogger logger, Guid conferenceId);
}