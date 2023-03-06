using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber
{
    public class BookingQueueSubscriberFunction
    {
        private readonly IMessageHandlerFactory _messageHandlerFactory;

        public BookingQueueSubscriberFunction(IMessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        [FunctionName("BookingQueueSubscriberFunction")]
        public async Task Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]
            string bookingQueueItem,
            ILogger log)
        {
            log.LogInformation("Processing message {BookingQueueItem}", bookingQueueItem);
            // get handler
            EventMessage eventMessage;
            try
            {
                eventMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);
            }
            catch (Exception e)
            {
                log.LogCritical(e, "Unable to deserialize into EventMessage \r\n {BookingQueueItem}", bookingQueueItem);
                throw;
            }

            var handler = _messageHandlerFactory.Get(eventMessage.IntegrationEvent);
            log.LogInformation("using handler {Handler}", handler.GetType());

            await handler.HandleAsync(eventMessage.IntegrationEvent);
            log.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
                eventMessage.IntegrationEvent);
        }
    }
}
