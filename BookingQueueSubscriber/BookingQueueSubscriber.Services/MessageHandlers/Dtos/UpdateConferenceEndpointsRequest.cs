using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers.Dtos
{
    public class UpdateConferenceEndpointsRequest
    {
        public IList<EndpointResponse> ExistingEndpoints { get; set; } = new List<EndpointResponse>();
        public IList<EndpointResponse> NewEndpoints { get; set; } = new List<EndpointResponse>();
    }
}
