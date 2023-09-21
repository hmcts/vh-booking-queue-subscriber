using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;

namespace BookingQueueSubscriber.UnitTests.Emails.MessageHandling.MultiDayHearing;

public class MultiDayHearingHandlerWithNewTemplateToggleOnTests : BaseEventHandlerTests
{
    private MultiDayHearingHandler _sut;
    
    [SetUp]
    public void Setup()
    {
        InitServices();

        _sut = new MultiDayHearingHandler(NotificationService, FeatureToggle, UserCreationAndNotificationService);
        
        FeatureToggle.PostMayTemplateToggle = true;
        NotificationService.EJudFeatureEnabled = false;
    }

    [Test]
    public async Task should_send_correct_templates_for_multi_day_event_and_users_are_treated_as_new()
    {
        // arrange
        UserService.CreateUser = true;
        var integrationEvent = new MultiDayHearingIntegrationEvent()
        {
            Hearing = HearingEventBuilders.CreateHearing(isMultiDay:true),
            Participants = HearingEventBuilders.CreateListOfParticipantOfEachType(),
            TotalDays = 2
        };

        // act
        await _sut.HandleAsync(integrationEvent);

        // assert
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipWelcome);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmation);
        
        NotificationService.NotificationRequests.Should().Contain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmationMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJudgeMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJohMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.CreateRepresentative);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationRepresentativeMultiDay);
    }
    
    [Test]
    public async Task should_send_correct_templates_for_multi_day_event_and_users_are_treated_as_existing()
    {
        // arrange
        UserService.CreateUser = false;
        var integrationEvent = new MultiDayHearingIntegrationEvent()
        {
            Hearing = HearingEventBuilders.CreateHearing(isMultiDay:true),
            Participants = HearingEventBuilders.CreateListOfParticipantOfEachType(),
            TotalDays = 2
        };

        // act
        await _sut.HandleAsync(integrationEvent);

        // assert
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipWelcome);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmation);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmationMultiDay);
        
        NotificationService.NotificationRequests.Should().Contain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.ExistingUserLipConfirmationMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJudgeMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJohMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationRepresentativeMultiDay);
    }
}