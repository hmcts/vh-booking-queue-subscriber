using System.Text;
using System.Net.Http;
using BookingQueueSubscriber.Common.Logging;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VideoApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.VideoWeb;

public class VideoWebService(HttpClient httpClient, ILogger<VideoWebService> logger) : IVideoWebService
{
    public async Task PushNewConferenceAdded(Guid conferenceId)
    {
        var path = $"internalevent/ConferenceAdded?conferenceId={conferenceId}";
        logger.PushNewConferenceAdded(conferenceId);
        var result = await httpClient.PostAsync(path, null);
        result.EnsureSuccessStatusCode();
    }

    public async Task PushParticipantsUpdatedMessage(Guid conferenceId, UpdateConferenceParticipantsRequest request)
    {
        var path = $"internalevent/ParticipantsUpdated?conferenceId={conferenceId}";
            
        logger.PushParticipantsUpdated(conferenceId);

        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });
            
        logger.ParticipantsUpdatedJson(json, $"{httpClient.BaseAddress}/{path}");

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync(path, httpContent);
        result.EnsureSuccessStatusCode();
        logger.ParticipantsUpdatedResponse(result);
    }
        
    public async Task PushAllocationToCsoUpdatedMessage(HearingAllocationNotificationRequest request)
    {
        var path = $"internalevent/AllocationHearings";
            
        logger.PushAllocationToCso();

        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync(path, httpContent);

        logger.AllocationResponse(result);
    }

    public async Task PushUnlinkedParticipantFromEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
    {
        var path = $"internalevent/UnlinkedParticipantFromEndpoint?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
        logger.PushUnlinkedParticipant(conferenceId, participantUserName, jvsEndpointName);
        await httpClient.PostAsync(path, null);
    }

    public async Task PushLinkedNewParticipantToEndpoint(Guid conferenceId, string participantUserName, string jvsEndpointName)
    {
        var path = $"internalevent/LinkedNewParticipantToEndpoint?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
        logger.PushLinkedParticipant(conferenceId, participantUserName, jvsEndpointName);
        await httpClient.PostAsync(path, null);
    }

    public async Task PushCloseConsultationBetweenEndpointAndParticipant(Guid conferenceId, string participantUserName, string jvsEndpointName)
    {
        var path = $"internalevent/CloseConsultationBetweenEndpointAndParticipant?conferenceId={conferenceId}&participant={participantUserName}&endpoint={jvsEndpointName}";
        logger.PushCloseConsultation(conferenceId, participantUserName, jvsEndpointName);
        await httpClient.PostAsync(path, null);
    }

    public async Task PushEndpointsUpdatedMessage(Guid conferenceId, UpdateConferenceEndpointsRequest request)
    {
        var path = $"internalevent/EndpointsUpdated?conferenceId={conferenceId}";

        logger.PushEndpointsUpdated(conferenceId);

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        });

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync(path, httpContent);
        result.EnsureSuccessStatusCode();
        logger.EndpointsUpdatedResponse(result);
    }
        
    public async Task PushHearingCancelledMessage(Guid conferenceId)
    {
        var path = $"internalevent/HearingCancelled?conferenceId={conferenceId}";
        logger.PushHearingCancelled(conferenceId);
        var result = await httpClient.PostAsync(path, null);
        result.EnsureSuccessStatusCode();
    }
        
    public async Task PushHearingDetailsUpdatedMessage(Guid conferenceId)
    {
        var path = $"internalevent/HearingDetailsUpdated?conferenceId={conferenceId}";
        logger.PushHearingDetailsUpdated(conferenceId);
        var result = await httpClient.PostAsync(path, null);
        result.EnsureSuccessStatusCode();
    }
}