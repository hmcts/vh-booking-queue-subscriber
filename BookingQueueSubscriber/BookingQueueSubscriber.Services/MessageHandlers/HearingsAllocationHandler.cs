using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.Services.VideoWeb.Models;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.Services.MessageHandlers;

public class HearingsAllocationHandler : IMessageHandler<HearingsAllocationIntegrationEvent>
{
    private readonly IVideoApiService _videoApiService;
    private readonly IVideoWebService _videoWebService;

    public HearingsAllocationHandler(IVideoWebService videoWebService, IVideoApiService videoApiService)
    {
        _videoWebService = videoWebService;
        _videoApiService = videoApiService;
    }

    public async Task HandleAsync(HearingsAllocationIntegrationEvent eventMessage)
    {
        var conferences = await GetConferenceForHearings(eventMessage.Hearings);
        var request = new AllocationUpdatedRequest()
        {
            AllocatedCsoUsername = eventMessage.AllocatedCso.Username,
            Conferences = conferences
        };
        await _videoWebService.PushAllocationUpdatedMessage(request);
    }

    // for each hearing get the conference details
    private async Task<List<ConferenceDetailsResponse>> GetConferenceForHearings(List<HearingDto> hearings)
    {
        var getTasks = hearings.Select(x => _videoApiService.GetConferenceByHearingRefId(x.HearingId)).ToList();
        var results = await Task.WhenAll(getTasks);
        return results.ToList();
    }

    public async Task HandleAsync(object integrationEvent)
    {
        await HandleAsync((HearingsAllocationIntegrationEvent)integrationEvent);
    }
}