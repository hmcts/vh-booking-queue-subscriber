using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingsApi.Client;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingQueueSubscriber.Services.MessageHandlers
{
    public class HearingReadyForVideoHandler : IMessageHandler<HearingIsReadyForVideoIntegrationEvent>
    {
        private readonly IVideoApiService _videoApiService;
        private readonly IVideoWebService _videoWebService;
        private readonly IBookingsApiClient _bookingsApiClient;

        public HearingReadyForVideoHandler(IVideoApiService videoApiService, 
            IVideoWebService videoWebService,
            IBookingsApiClient bookingsApiClient)
        {
            _videoApiService = videoApiService;
            _videoWebService = videoWebService;
            _bookingsApiClient = bookingsApiClient;
        }

        public async Task HandleAsync(HearingIsReadyForVideoIntegrationEvent eventMessage)
        {
            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(eventMessage.Hearing,
                eventMessage.Participants, eventMessage.Endpoints);
            var conferenceDetailsResponse = await _videoApiService.BookNewConferenceAsync(request);
            await _bookingsApiClient.UpdateBookingStatusAsync(eventMessage.Hearing.HearingId, new UpdateBookingStatusRequest
            { Status = UpdateBookingStatus.Created, UpdatedBy = "System" });
            await _videoWebService.PushNewConferenceAdded(conferenceDetailsResponse.Id);

        }

        async Task IMessageHandler.HandleAsync(object integrationEvent)
        {
            await HandleAsync((HearingIsReadyForVideoIntegrationEvent) integrationEvent);
        }
    }
}