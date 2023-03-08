using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using BookingsApi.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunctionTests;

public class AllocationMessageTests : MessageHandlerTestBase
{
    private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
    private VideoApiServiceFake _videoApiService;
    private VideoWebServiceFake _videoWebService;
    private BookingQueueSubscriberFunction _sut;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _videoApiService = (VideoApiServiceFake) _serviceProvider.GetService<IVideoApiService>();
        _videoWebService = (VideoWebServiceFake) _serviceProvider.GetService<IVideoWebService>();
        _sut = new BookingQueueSubscriberFunction(
            new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
    }
    
    [SetUp]
    public void Setup()
    {
        ParticipantId = Guid.NewGuid();
        HearingId = Guid.NewGuid();
        VideoApiServiceMock = new Mock<IVideoApiService>();

        VideoWebServiceMock = new Mock<IVideoWebService>();
        UserServiceMock = new Mock<IUserService>();
        NotificationServiceMock = new Mock<INotificationService>();
        UserCreationAndNotificationMock = new Mock<IUserCreationAndNotification>();
        BookingsApiClientMock = new Mock<IBookingsApiClient>();
    }

    [TearDown]
    public void TearDown()
    {
        _videoApiService.ClearRequests();
    }

    [Test]
    public async Task should_handle_hearing_allocation_integration_event()
    {
        var conference1 = new ConferenceDetailsResponse()
        {
            HearingId = Guid.Parse("a83eeeda-30f5-4202-960e-873204cf1205"), Id = Guid.NewGuid(), CaseName = "Test",
            CaseNumber = "AutoTest"
        };
        var conference2 = new ConferenceDetailsResponse()
        {
            HearingId = Guid.Parse("9b6ed100-8162-4d6a-8d65-acd005f8dda6"), Id = Guid.NewGuid(), CaseName = "Test",
            CaseNumber = "AutoTest"
        };
        VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(conference1.Id, It.IsAny<bool>())).ReturnsAsync(conference1);
        VideoApiServiceMock.Setup(x => x.GetConferenceByHearingRefId(conference2.Id, It.IsAny<bool>())).ReturnsAsync(conference2);
        
        const string message = @"{
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': '635b98f9-c187-4b8c-9fc7-848cc194a970',
              'timestamp': '2023-03-08T15:23:38.717934Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingsAllocationIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearings': [
                  {
                    '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
                    'hearing_id': 'a83eeeda-30f5-4202-960e-873204cf1205',
                    'group_id': '00000000-0000-0000-0000-000000000001',
                    'scheduled_date_time': '2023-03-08T11:45:00Z',
                    'scheduled_duration': 80,
                    'case_type': 'Generic',
                    'case_number': 'AutoTest',
                    'case_name': 'Test',
                    'hearing_venue_name': 'Birmingham Civil and Family Justice Centre',
                    'record_audio': true,
                    'hearing_type': 'Automated Test'
                  },
                  {
                    '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
                    'hearing_id': '9b6ed100-8162-4d6a-8d65-acd005f8dda6',
                    'group_id': '00000000-0000-0000-0000-000000000001',
                    'scheduled_date_time': '2023-03-08T11:45:00Z',
                    'scheduled_duration': 80,
                    'case_type': 'Generic',
                    'case_number': 'AutoTest',
                    'case_name': 'Test',
                    'hearing_venue_name': 'Birmingham Civil and Family Justice Centre',
                    'record_audio': true,
                    'hearing_type': 'Automated Test'
                  }
                ],
                'allocated_cso': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.JusticeUserDto, BookingsApi.Infrastructure.Services',
                  'user_id': '00000000-0000-0000-0000-000000000000',
                  'username': 'contact@email.com',
                  'user_role_name': 'Video Hearings Team Lead'
                }
              }
            }
            ";
        
        await _sut.Run(message, new LoggerFake());
        _videoWebService.PushAllocationToCsoUpdatedMessageCount.Should().Be(1);
    }
}