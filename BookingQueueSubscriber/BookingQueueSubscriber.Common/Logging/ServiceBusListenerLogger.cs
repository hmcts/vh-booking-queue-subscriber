using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public static partial class ServiceBusListenerLogger
{
    [LoggerMessage(1001, LogLevel.Information, "Processing message {BookingQueueItem}")]
    public static partial void ProcessingMessage(this ILogger logger, string bookingQueueItem);

    [LoggerMessage(1002, LogLevel.Debug, "Using handler {HandlerType}")]
    public static partial void UsingHandler(this ILogger logger, Type handlerType);
    
    [LoggerMessage(1003, LogLevel.Information, "Process message {MessageId} - {IntegrationEvent}")]
    public static partial void ProcessMessage(this ILogger logger, Guid messageId, object integrationEvent);
    
    [LoggerMessage(1004, LogLevel.Information, "Stopping service bus processor")]
    public static partial void StoppingServiceBusProcessor(this ILogger logger);
    
    [LoggerMessage(1005, LogLevel.Information, "Starting service bus processor")]
    public static partial void StartingServiceBusProcessor(this ILogger logger);
}