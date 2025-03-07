﻿using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb
{
    public interface IVideoWebService
    {
        Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request);
        Task PushEndpointsUpdatedMessage(Guid conferenceId, UpdateConferenceEndpointsRequest request);
        Task PushNewConferenceAdded(Guid conferenceId);
        Task PushAllocationToCsoUpdatedMessage(HearingAllocationNotificationRequest request);
        Task PushUnlinkedParticipantFromEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName);
        Task PushLinkedNewParticipantToEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName);
        Task PushCloseConsultationBetweenEndpointAndParticipant(Guid conferenceId, string participantUserName, string jvsEndpointName);
        Task PushHearingCancelledMessage(Guid conferenceId);
        Task PushHearingDetailsUpdatedMessage(Guid conferenceId);
    }
}
