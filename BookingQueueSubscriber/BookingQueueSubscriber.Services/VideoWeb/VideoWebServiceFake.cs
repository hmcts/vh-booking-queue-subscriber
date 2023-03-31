using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    [ExcludeFromCodeCoverage]
    public class VideoWebServiceFake: IVideoWebService
    {
        public int PushParticipantsUpdatedMessageCount { get; set; }
        public int PushNewConferenceAddedMessageCount { get; set; }
        
        public int PushAllocationToCsoUpdatedMessageCount { get; set; }

        public int PushEndpointsUpdatedMessageCount { get; set; }

        public Task PushEndpointsUpdatedMessage(Guid conferenceId, UpdateConferenceEndpointsRequest request)
        {
            PushEndpointsUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

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
        
        public Task PushAllocationToCsoUpdatedMessage(AllocationHearingsToCsoRequest request)
        {
            PushAllocationToCsoUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
