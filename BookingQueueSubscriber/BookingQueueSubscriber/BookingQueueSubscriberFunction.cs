namespace BookingQueueSubscriber
{
    public class BookingQueueSubscriberFunction
    {
        private readonly IMessageHandlerFactory _messageHandlerFactory;
        private readonly ILogger<BookingQueueSubscriberFunction> _logger;

        public BookingQueueSubscriberFunction(IMessageHandlerFactory messageHandlerFactory, ILogger<BookingQueueSubscriberFunction> logger)
        {
            _messageHandlerFactory = messageHandlerFactory; 
            _logger = logger;
        }

        [FunctionName("BookingQueueSubscriberFunction")]
        public async Task Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]
            string bookingQueueItem)
        {
            _logger.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
            // get handler
            EventMessage eventMessage;
            try
            {
                eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unable to deserialize into EventMessage \r\n {BookingQueueItem}", bookingQueueItem);
                throw;
            }

            var handler = _messageHandlerFactory.Get(eventMessage.IntegrationEvent);
            _logger.LogInformation("using handler {Handler}", handler.GetType());

            await handler.HandleAsync(eventMessage.IntegrationEvent);
            _logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
                eventMessage.IntegrationEvent);
        }
    }
}
