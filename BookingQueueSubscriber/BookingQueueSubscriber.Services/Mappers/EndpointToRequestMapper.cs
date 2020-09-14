using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;

namespace BookingQueueSubscriber.Services.Mappers
{
    public static class EndpointToRequestMapper
    {
        public static AddEndpointRequest MapToRequest(EndpointDto source)
        {
            return new AddEndpointRequest
            {
                SipAddress = source.Sip,
                DisplayName = source.DisplayName,
                Pin = source.Pin,
                DefenceAdvocate = source.DefenceAdvocateUsername
            };
        }
    }
}