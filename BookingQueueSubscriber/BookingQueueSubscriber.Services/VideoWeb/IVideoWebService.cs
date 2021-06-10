using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public interface IVideoWebService
    {
        Task PushParticipantsAddedMessage(Guid conferenceId, AddParticipantsToConferenceRequest request);
    }
}
