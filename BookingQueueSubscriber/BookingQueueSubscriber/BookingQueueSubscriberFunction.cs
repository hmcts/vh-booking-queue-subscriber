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
            [Inject]IMessageHandlerFactory messageHandlerFactory)
        {
            // get handler
            var bookingsMessage = MessageSerializer.Deserialise<EventMessage>(bookingQueueItem);
            var handler = messageHandlerFactory.Get(bookingsMessage.IntegrationEvent);
            log.LogDebug($"using handler {handler.GetType()}");

            // execute handler
            await handler.HandleAsync(bookingsMessage.IntegrationEvent).ConfigureAwait(false);
            log.LogInformation($"Process message {bookingsMessage.Id} - {bookingsMessage.IntegrationEvent}");
        }
    }
}
