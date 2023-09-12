using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
            PushParticipantsUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }
        
        public Task PushAllocationToCsoUpdatedMessage(AllocationHearingsToCsoRequest request)
        {
            PushAllocationToCsoUpdatedMessageCount++;
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task PushUnlinkedParticipantFromEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task PushLinkedNewParticipantToEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public Task PushCloseConsultationBetweenEndpointAndParticipant(Guid conferenceId, string participantUserName, string jvsEndpointName)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
