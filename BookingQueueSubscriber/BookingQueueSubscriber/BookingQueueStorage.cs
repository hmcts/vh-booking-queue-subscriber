using System.Threading.Tasks;
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber;

public class BookingQueueStorage
{
    private readonly IMessageHandlerFactory _messageHandlerFactory;

    public BookingQueueStorage(IMessageHandlerFactory messageHandlerFactory)
    {
        _messageHandlerFactory = messageHandlerFactory;
    }

    [FunctionName("BookingQueueStorage")]
    public async Task RunAsync([QueueTrigger("%queueName%", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger logger)
    {
        // get handler
        EventMessage eventMessage;
        try
        {
            eventMessage = MessageSerializer.Deserialise<EventMessage>(myQueueItem);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unable to deserialize into EventMessage \r\n {BookingQueueItem}", myQueueItem);
            throw;
        }

        var handler = _messageHandlerFactory.Get(eventMessage.IntegrationEvent);
        logger.LogInformation("using handler {Handler}", handler.GetType());

        await handler.HandleAsync(eventMessage.IntegrationEvent);
        logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
            eventMessage.IntegrationEvent);
    }
}