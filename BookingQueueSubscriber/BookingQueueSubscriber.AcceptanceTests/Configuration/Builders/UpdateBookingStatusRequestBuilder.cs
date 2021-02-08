using BookingQueueSubscriber.AcceptanceTests.Configuration.Data;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;

namespace BookingQueueSubscriber.AcceptanceTests.Configuration.Builders
{
    public class UpdateBookingStatusRequestBuilder
    {
        private readonly UpdateBookingStatusRequest _request;

        public UpdateBookingStatusRequestBuilder()
        {
            _request = new UpdateBookingStatusRequest { Status = UpdateBookingStatus.Created };
        }

        public UpdateBookingStatusRequestBuilder WithStatus(UpdateBookingStatus status)
        {
            _request.CancelReason = HearingData.CANCELLATION_REASON;
            _request.Status = status;
            return this;
        }

        public UpdateBookingStatusRequestBuilder UpdatedBy(string username)
        {
            _request.UpdatedBy = username;
            return this;
        }

        public UpdateBookingStatusRequest Build()
        {
            return _request;
        }
    }
}
