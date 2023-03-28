using System;
using System.Net;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebServiceFake: IVideoWebService
    {
        public int PushParticipantsUpdatedMessageCount { get; set; }
        public int PushNewConferenceAddedMessageCount { get; set; }
        
        public int PushAllocationToCsoUpdatedMessageCount { get; set; }

        public Task PushNewConferenceAdded(Guid conferenceId)
        {
            PushNewConferenceAddedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            PushParticipantsUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task PushAllocationUpdatedMessage(AllocationUpdatedRequest request)
        {
            PushAllocationToCsoUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.Accepted);
        }
    }
}
