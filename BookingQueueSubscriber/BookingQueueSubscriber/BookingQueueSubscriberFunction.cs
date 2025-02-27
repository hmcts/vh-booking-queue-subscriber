// namespace BookingQueueSubscriber
// {
//     public class BookingQueueSubscriberFunction(IMessageHandlerFactory messageHandlerFactory, ILogger<BookingQueueSubscriberFunction> logger)
//     {
//         [Function("BookingQueueSubscriberFunction")]
//         public async Task Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]
//             string bookingQueueItem)
//         {
//             logger.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
//             // get handler
//             var eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);
//
//             var handler = messageHandlerFactory.Get(eventMessage.IntegrationEvent);
//             logger.LogDebug("using handler {Handler}", handler.GetType());
//
//             await handler.HandleAsync(eventMessage.IntegrationEvent);
//             logger.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
//                 eventMessage.IntegrationEvent);
//         }
//     }
// }
