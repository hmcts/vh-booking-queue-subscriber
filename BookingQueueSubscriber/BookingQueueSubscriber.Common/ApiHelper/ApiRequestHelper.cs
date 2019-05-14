using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BookingQueueSubscriber.Common.ApiHelper
{
    public static class ApiRequestHelper
    {
        public static string SerialiseRequestToSnakeCaseJson(object request)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            
            return JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }
        
        public static T DeserialiseSnakeCaseJsonToResponse<T>(string response)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            
            return JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            });
        }
    }
}