using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.UnitTests.Emails.MessageHandling.HearingReadyForVideo;

public class HearingReadyForVideoHandlerWithNewTemplateToggleOffTests : BaseEventHandlerTests
{
    private HearingReadyForVideoHandler _sut;

    [SetUp]
    public void Setup()
    {
        InitServices();

        _sut = new HearingReadyForVideoHandler(VideoApiService, VideoWebService, UserCreationAndNotificationService,
            BookingsApi, NotificationService, FeatureToggle);

        FeatureToggle.PostMayTemplateToggle = false;
        NotificationService.EJudFeatureEnabled = false;
    }

    [Test]
    public async Task should_send_correct_templates_for_single_day_hearing()
    {
        // arrange
        var integrationEvent = new HearingIsReadyForVideoIntegrationEvent()
        {
            Hearing = HearingEventBuilders.CreateHearing(),
            Participants = HearingEventBuilders.CreateListOfParticipantOfEachType(),
            Endpoints = new List<EndpointDto>()
        };
        // act
        await _sut.HandleAsync(integrationEvent);

        // assert
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJudge);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJoh);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationRepresentative);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLip);
    }
    
    [Test]
    public async Task should_send_correct_templates_for_multi_day_hearing()
    {
        // arrange
        var integrationEvent = new HearingIsReadyForVideoIntegrationEvent()
        {
            Hearing = HearingEventBuilders.CreateHearing(isMultiDay:true),
            Participants = HearingEventBuilders.CreateListOfParticipantOfEachType(),
            Endpoints = new List<EndpointDto>()
        };
        // act
        await _sut.HandleAsync(integrationEvent);

        // assert
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJudge);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJoh);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationRepresentative);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLip);
    }
}