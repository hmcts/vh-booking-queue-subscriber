using System;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public interface IVideoWebService
    {
        Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request);
        Task PushNewConferenceAdded(Guid conferenceId);

        Task PushAllocationToCsoUpdatedMessage(AllocationHearingsToCsoRequest request);
    }
}
