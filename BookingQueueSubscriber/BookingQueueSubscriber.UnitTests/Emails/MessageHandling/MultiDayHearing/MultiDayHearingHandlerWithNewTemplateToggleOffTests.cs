using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;

namespace BookingQueueSubscriber.UnitTests.Emails.MessageHandling.MultiDayHearing;

public class MultiDayHearingHandlerWithNewTemplateToggleOffTests : BaseEventHandlerTests
{
    private MultiDayHearingHandler _sut;
    
    [SetUp]
    public void Setup()
    {
        InitServices();

        _sut = new MultiDayHearingHandler(NotificationService, FeatureToggle, UserCreationAndNotificationService);
        
        FeatureToggle.PostMayTemplateToggle = false;
        NotificationService.EJudFeatureEnabled = false;
    }

    [Test]
    public async Task should_send_correct_templates_for_multi_day_event()
    {
        // arrange
        var integrationEvent = new MultiDayHearingIntegrationEvent()
        {
            Hearing = HearingEventBuilders.CreateHearing(),
            Participants = HearingEventBuilders.CreateListOfParticipantOfEachType(),
            TotalDays = 2
        };
        

        // act
        await _sut.HandleAsync(integrationEvent);

        // assert
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJudgeMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationJohMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLipMultiDay);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationRepresentativeMultiDay);
    }
}