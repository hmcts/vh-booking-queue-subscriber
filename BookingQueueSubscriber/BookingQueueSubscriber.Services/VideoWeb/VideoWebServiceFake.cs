using System;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebServiceFake: IVideoWebService
    {
        public int PushParticipantsUpdatedMessageCount { get; set; }

        public Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request)
        {
            PushParticipantsUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
