namespace BookingQueueSubscriber
{
    public class BookingQueueSubscriberFunction(IMessageHandlerFactory messageHandlerFactory, ILogger<BookingQueueSubscriberFunction> logger)
    {
        [Function("BookingQueueSubscriberFunction")]
        public async Task Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]
            string bookingQueueItem)
        {
            logger.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
            // get handler
            EventMessage eventMessage;
            try
            {
                eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Unable to deserialize into EventMessage \r\n {BookingQueueItem}", bookingQueueItem);
                throw;
            }

            var handler = messageHandlerFactory.Get(eventMessage.IntegrationEvent);
            logger.LogInformation("using handler {Handler}", handler.GetType());

            await handler.HandleAsync(eventMessage.IntegrationEvent);
            logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
                eventMessage.IntegrationEvent);
        }
    }
}
