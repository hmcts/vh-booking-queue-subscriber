using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace BookingQueueSubscriber
{
    public static class BookingQueueSubscriberFunction
    {
        [FunctionName("BookingQueueSubscriberFunction")]
        public static async Task Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]
            string bookingQueueItem,
            ILogger log,
            [Inject] IMessageHandlerFactory messageHandlerFactory)
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

            var handler = messageHandlerFactory.Get(eventMessage.IntegrationEvent);
            log.LogInformation("using handler {Handler}", handler.GetType());

            // execute handler
            await handler.HandleAsync(eventMessage.IntegrationEvent);
            log.LogInformation("Process message {EventMessageId} - {EventMessageIntegrationEvent}", eventMessage.Id,
                eventMessage.IntegrationEvent);
        }
    }
}
