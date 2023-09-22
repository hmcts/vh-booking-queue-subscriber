using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.UnitTests.Emails.MessageHandling.HearingReadyForVideo;

public class HearingReadyForVideoHandlerWithNewTemplateToggleOnTests : BaseEventHandlerTests
{
    private HearingReadyForVideoHandler _sut;

    [SetUp]
    public void Setup()
    {
        InitServices();

        _sut = new HearingReadyForVideoHandler(VideoApiService, VideoWebService, UserCreationAndNotificationService,
            BookingsApi, NotificationService, FeatureToggle);

        FeatureToggle.PostMayTemplateToggle = true;
        NotificationService.EJudFeatureEnabled = false;
    }

    [Test]
    public async Task should_send_correct_templates_for_single_day_hearing_and_users_are_treated_as_new()
    {
        // arrange
        UserService.CreateUser = true;
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
            x.NotificationType == NotificationApi.Contract.NotificationType.CreateRepresentative);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipWelcome);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmation);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLip);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.CreateIndividual);
    }
    
    [Test]
    public async Task should_send_correct_templates_for_single_day_hearing_and_users_are_treated_as_existing()
    {
        // arrange
        UserService.CreateUser = false;
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
            x.NotificationType == NotificationApi.Contract.NotificationType.ExistingUserLipConfirmation);
    }
    
    [Test]
    public async Task should_send_correct_templates_for_multi_day_hearing_and_users_are_treated_as_new()
    {
        // arrange
        UserService.CreateUser = true;
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
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipWelcome);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmation);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLip);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.CreateIndividual);
    }
}