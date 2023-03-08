using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BookingQueueSubscriber.Services
{
    public static class MessageSerializer
    {
        public static T Deserialise<T>(string message)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = new EventMessageTypeBinder(Assembly.GetExecutingAssembly())
            };

            return JsonConvert.DeserializeObject<T>(message, settings);
        }
        
        public static string Serialise(object message)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.Objects,
            };

            return JsonConvert.SerializeObject(message, settings);
        }
    }
}