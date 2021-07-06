using System;
using System.Net;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebServiceFake: IVideoWebService
    {
        public int PushParticipantsAddedMessageCount { get; set; }

        public Task PushParticipantsAddedMessage(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            PushParticipantsAddedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
