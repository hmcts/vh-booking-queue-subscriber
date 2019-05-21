using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Common.ApiHelper;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
            log.LogDebug($"message: {bookingQueueItem}");
            foreach (var messageHandler in messageHandlerFactory.MessageHandlers)
            {
                log.LogDebug($"handler: {messageHandler}");
            }
            // get handler
            var bookingsMessage =
                ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<BookingsMessage>(bookingQueueItem);
            log.LogInformation($"event type {bookingsMessage.IntegrationEvent.EventType}");
            log.LogInformation($"message handler factory null {messageHandlerFactory == null}");
            log.LogInformation($"message handler factory {messageHandlerFactory}");
            var handler = messageHandlerFactory.Get(bookingsMessage.IntegrationEvent.EventType);
            log.LogDebug($"using handler {handler.GetType()}");
            
            // deserialize into correct contract
            var eventRaw = JObject.Parse(bookingQueueItem)["integration_event"].ToString();
            var bodyType = handler.BodyType;
            var typedEvent = DeserialiseSnakeCaseJsonToIntegrationEvent(eventRaw, bodyType);
            log.LogDebug($"deserialising to {bodyType}");
            log.LogDebug($"{eventRaw}");
            
            // execute handler
            await handler.HandleAsync(typedEvent).ConfigureAwait(false);
            log.LogInformation($"Process message {bookingsMessage.Id} - {bookingsMessage.IntegrationEvent}");
        }
        
        private static IntegrationEvent DeserialiseSnakeCaseJsonToIntegrationEvent(string response, Type type) 
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };

            var dto = JsonConvert.DeserializeObject(response, type, settings);
            return (IntegrationEvent) dto;
        }
    }
}
