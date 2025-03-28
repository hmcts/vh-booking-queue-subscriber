﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using BookingQueueSubscriber.Services.VideoWeb.Models;
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
        
        public Task PushAllocationToCsoUpdatedMessage(HearingAllocationNotificationRequest request)
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
        
        public Task PushHearingCancelledMessage(Guid conferenceId)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }
        
        public Task PushHearingDetailsUpdatedMessage(Guid conferenceId)
        {
            return Task.FromResult(HttpStatusCode.OK);
        }

        public void ClearRequests()
        {
            PushParticipantsUpdatedMessageCount = PushNewConferenceAddedMessageCount =
                PushAllocationToCsoUpdatedMessageCount = PushEndpointsUpdatedMessageCount = 0;
        }
    }
}
