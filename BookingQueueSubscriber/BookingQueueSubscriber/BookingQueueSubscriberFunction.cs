using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber
{
    public static class BookingQueueSubscriberFunction
    {
        [FunctionName("BookingQueueSubscriberFunction")]
        public static void Run([ServiceBusTrigger("%queueName%", Connection = "ServiceBusConnection")]string bookingQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {bookingQueueItem}");
        }
    }
}
