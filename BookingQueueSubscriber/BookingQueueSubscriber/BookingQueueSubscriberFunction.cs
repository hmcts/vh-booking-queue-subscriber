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
            var eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);

            var handler = _messageHandlerFactory.Get(eventMessage.IntegrationEvent);
            _logger.LogDebug("using handler {Handler}", handler.GetType());

            await handler.HandleAsync(eventMessage.IntegrationEvent);
            _logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
                eventMessage.IntegrationEvent);
        }
    }
}
