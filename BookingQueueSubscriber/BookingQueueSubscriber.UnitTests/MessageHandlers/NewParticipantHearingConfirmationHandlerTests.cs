using System.Net;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using VideoApi.Client;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers;

public class NewParticipantHearingConfirmationHandlerTests : MessageHandlerTestBase
{
    [Test]
    public async Task should_poll_video_api_for_response_then_throw_error()
    {
        var messageHandler = new NewParticipantHearingConfirmationHandler(
            UserServiceMock.Object,
            NotificationApiClientMock.Object, 
            BookingsApiClientMock.Object, 
            VideoApiServiceMock.Object);

        UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new User() { UserName = "username"});
        
        //video mock should throw not found exception
        VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null));
        
        var integrationEvent = new NewParticipantHearingConfirmationEvent
        {
            HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto()
            {
                HearingId = HearingId,
                ParticipantId = ParticipantId,
                ContactEmail = "email@email.com",
                FirstName = "John",
                LastName = "Smith",
                UserRole = "Individual",
                CaseName = "Case Name",
                CaseNumber = "1234567890",
                ScheduledDateTime = DateTime.UtcNow
            }
        };
        //assert that message handler throws exception
        Assert.ThrowsAsync<VideoApiException>(() => messageHandler.HandleAsync(integrationEvent));
    }
    
    [Test]
    public async Task should_poll_video_api_for_response_then_return_it_after_initial_error()
    {
        var messageHandler = new NewParticipantHearingConfirmationHandler(
            UserServiceMock.Object,
            NotificationApiClientMock.Object, 
            BookingsApiClientMock.Object, 
            VideoApiServiceMock.Object);

        UserServiceMock.Setup(x => x.CreateNewUserForParticipantAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new User() { UserName = "username"});
        
        //video mock should throw not found exception, then return conference on second iteration
        VideoApiServiceMock.SetupSequence(x => x.GetConferenceByHearingRefId(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ThrowsAsync(new VideoApiException("Conference not found", (int)HttpStatusCode.NotFound, "Conference not found", null, null))
            .ReturnsAsync(new ConferenceDetailsResponse()
            {
                Participants = new List<ParticipantDetailsResponse>()
                {
                    new ParticipantDetailsResponse()
                    {
                        Id = ParticipantId,
                        ContactEmail = "email@email.com"
                    }
                }
            });
            
        
        var integrationEvent = new NewParticipantHearingConfirmationEvent
        {
            HearingConfirmationForParticipant = new HearingConfirmationForParticipantDto()
            {
                HearingId = HearingId,
                ParticipantId = ParticipantId,
                ContactEmail = "email@email.com",
                FirstName = "John",
                LastName = "Smith",
                UserRole = "Individual",
                CaseName = "Case Name",
                CaseNumber = "1234567890",
                ScheduledDateTime = DateTime.UtcNow
            }
        };
        await messageHandler.HandleAsync(integrationEvent);
        //assert message handler does not throw exception
        Assert.Pass();
    }
    
    
}