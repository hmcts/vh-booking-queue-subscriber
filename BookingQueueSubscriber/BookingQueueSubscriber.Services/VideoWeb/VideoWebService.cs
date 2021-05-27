using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public class VideoWebService : IVideoWebService
    {
        VideoWebService(IHttpClient httpClient, ILogger<VideoWebService> logger)
        public Task PushAddParticipantsMessage(Guid conferenceId, AddParticipantsToConferenceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
