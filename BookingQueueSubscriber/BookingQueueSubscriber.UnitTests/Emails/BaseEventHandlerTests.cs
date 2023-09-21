using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using BookingsApi.Client;

namespace BookingQueueSubscriber.UnitTests.Emails;

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
        
        NotificationService.ClearRequests();
    }
}