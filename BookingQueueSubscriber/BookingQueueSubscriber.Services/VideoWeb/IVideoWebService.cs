using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public interface IVideoWebService
    {
        Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request);
        Task PushNewConferenceAdded(Guid conferenceId);
        Task PushAllocationUpdatedMessage(AllocationUpdatedRequest request);
    }
}
