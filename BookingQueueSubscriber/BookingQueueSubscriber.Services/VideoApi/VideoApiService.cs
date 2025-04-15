using System.Net;
using BookingQueueSubscriber.Common.Logging;
using VideoApi.Contract.Requests;
using VideoApi.Client;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.VideoApi;

public class VideoApiService(IVideoApiClient apiClient, ILogger<VideoApiService> logger) : IVideoApiService
{
    public Task<ConferenceDetailsResponse> BookNewConferenceAsync(BookNewConferenceRequest request)
    {
        logger.BookingNewConference(request.HearingRefId);
        return apiClient.BookNewConferenceAsync(request);
    }

    public Task UpdateConferenceAsync(UpdateConferenceRequest request)
    {
        logger.UpdatingConference(request.HearingRefId);
        return apiClient.UpdateConferenceAsync(request);
    }

    public Task DeleteConferenceAsync(Guid conferenceId)
    {
        logger.DeletingConference(conferenceId);
        return apiClient.RemoveConferenceAsync(conferenceId);
    }

    public async Task<ConferenceDetailsResponse> GetConferenceByHearingRefId(Guid hearingRefId, bool includeClosed = false)
    {
        logger.GettingConferenceByHearing(hearingRefId);
        var request = new GetConferencesByHearingIdsRequest { HearingRefIds = [hearingRefId], IncludeClosed = includeClosed };
        var conferences = await apiClient.GetConferenceDetailsByHearingRefIdsAsync(request);
        return conferences.FirstOrDefault();
    }

    public Task<ICollection<EndpointResponse>> GetEndpointsForConference(Guid conferenceId)
    {
        logger.GettingEndpoints(conferenceId);
        return apiClient.GetEndpointsForConferenceAsync(conferenceId);
    }

    public Task AddParticipantsToConference(Guid conferenceId, AddParticipantsToConferenceRequest request)
    {
        logger.AddingParticipants(conferenceId);
        return apiClient.AddParticipantsToConferenceAsync(conferenceId, request);
    }

    public Task RemoveParticipantFromConference(Guid conferenceId, Guid participantId)
    {
        logger.RemovingParticipant( participantId, conferenceId);
        return apiClient.RemoveParticipantFromConferenceAsync(conferenceId, participantId);
    }

    public Task UpdateConferenceParticipantsAsync(Guid conferenceId, UpdateConferenceParticipantsRequest request)
    {
        logger.UpdatingParticipants(conferenceId, request);
        return apiClient.UpdateConferenceParticipantsAsync(conferenceId, request);
    }

    public Task UpdateParticipantDetails(Guid conferenceId, Guid participantId,
        UpdateParticipantRequest request)
    {
        logger.UpdatingParticipant(participantId, conferenceId);
        return apiClient.UpdateParticipantDetailsAsync(conferenceId, participantId, request);
    }

    public Task AddEndpointToConference(Guid conferenceId, AddEndpointRequest request)
    {
        logger.AddingEndpoint(conferenceId);
        return apiClient.AddEndpointToConferenceAsync(conferenceId, request);
    }

    public Task RemoveEndpointFromConference(Guid conferenceId, string sip)
    {
        logger.RemovingEndpoint(sip, conferenceId);
        return apiClient.RemoveEndpointFromConferenceAsync(conferenceId, sip);
    }

    public Task UpdateEndpointInConference(Guid conferenceId, string sip, UpdateEndpointRequest request)
    {
        logger.UpdatingEndpoint(sip, conferenceId);
        return apiClient.UpdateEndpointInConferenceAsync(conferenceId, sip, request);
    }

    public Task CloseConsultation(Guid conferenceId, Guid participantId)
    {
        logger.ClosingConsultation(conferenceId);
        return apiClient.LeaveConsultationAsync(new LeaveConsultationRequest{ConferenceId = conferenceId, ParticipantId = participantId});
    }

    private Task UpdateParticipantUsername(Guid participantId, string username)
    {
        logger.UpdatingParticipantUsername(participantId);
        return apiClient.UpdateParticipantUsernameAsync(participantId, new UpdateParticipantUsernameRequest { Username = username });
    }

    public async Task UpdateParticipantUsernameWithPolling(Guid hearingId, string username, string contactEmail)
    {
        var pollCount = 0;
            
        ConferenceDetailsResponse conferenceResponse;
        do {
            conferenceResponse = await PollForConferenceDetails(); 
            pollCount++;
        } while (conferenceResponse == null);

        var participant = conferenceResponse.Participants.Single(x => x.ContactEmail == contactEmail);
        await UpdateParticipantUsername(participant.Id, username);
            
        async Task<ConferenceDetailsResponse> PollForConferenceDetails()
        {
            try
            {
                return await GetConferenceByHearingRefId(hearingId, true);
            }
            catch (VideoApiException e)
            {
                if(pollCount >= 3) 
                    throw;
                
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    return null;
                }

                throw;
            }
        }
    }

    public Task<ICollection<ConferenceDetailsResponse>> GetConferencesByHearingRefIdsAsync(
        GetConferencesByHearingIdsRequest request)
    {
        return apiClient.GetConferenceDetailsByHearingRefIdsAsync(request);
    }
}