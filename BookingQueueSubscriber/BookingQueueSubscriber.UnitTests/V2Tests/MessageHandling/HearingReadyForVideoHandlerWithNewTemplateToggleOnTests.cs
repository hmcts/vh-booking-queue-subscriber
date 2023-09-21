using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.IntegrationEvents;
using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using BookingsApi.Client;

namespace BookingQueueSubscriber.UnitTests.V2Tests.MessageHandling;

public abstract class BaseEventHandlerTests
{
    protected VideoApiServiceFake VideoApiService;
    protected VideoWebServiceFake VideoWebService;
    protected NotificationServiceFake NotificationService;
    protected IUserCreationAndNotification UserCreationAndNotificationService;
    protected BookingsApiClientFake BookingsApi;
    protected UserServiceFake UserService;
    protected FeatureTogglesClientFake FeatureToggle;
    
    protected void InitServices()
    {
        var serviceProvider = ServiceProviderFactory.ServiceProvider;
        
        VideoApiService = (VideoApiServiceFake) serviceProvider.GetService<IVideoApiService>();
        VideoWebService = (VideoWebServiceFake) serviceProvider.GetService<IVideoWebService>();
        NotificationService = (NotificationServiceFake) serviceProvider.GetService<INotificationService>();
        UserCreationAndNotificationService = serviceProvider.GetService<IUserCreationAndNotification>();
        BookingsApi = (BookingsApiClientFake)serviceProvider.GetService<IBookingsApiClient>();
        FeatureToggle = (FeatureTogglesClientFake)serviceProvider.GetService<IFeatureToggles>();
        UserService = (UserServiceFake)serviceProvider.GetService<IUserService>();
    }
}

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
        NotificationService.ClearRequests();
    }

    [Test]
    public async Task should_send_correct_templates_and_users_are_treated_as_new()
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
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipWelcome);
        
        NotificationService.NotificationRequests.Should().ContainSingle(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.NewUserLipConfirmation);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.HearingConfirmationLip);
        
        NotificationService.NotificationRequests.Should().NotContain(x =>
            x.NotificationType == NotificationApi.Contract.NotificationType.CreateIndividual);
    }
    
    [Test]
    public async Task should_send_correct_templates_and_users_are_treated_as_existing()
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
}

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
    public async Task should_send_correct_templates()
    {
        // arrange
        var integrationEvent =new HearingIsReadyForVideoIntegrationEvent()
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
}