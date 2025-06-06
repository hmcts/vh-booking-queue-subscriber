﻿using System.Globalization;
using Azure.Messaging.ServiceBus;
using BookingQueueSubscriber.HostedServices;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using BookingQueueSubscriber.Wrappers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace BookingQueueSubscriber.UnitTests.HostedServices.ServiceBusListenerTests
{
    public class HandleMessageTests
    {
        private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
        private VideoApiServiceFake _videoApiService;
        private VideoWebServiceFake _videoWebService;
        private NotificationApiClientFake _notificationApiClient;
        private UserServiceFake _userService;
        private ServiceBusListener _serviceBusListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _videoApiService = (VideoApiServiceFake)_serviceProvider.GetService<IVideoApiService>();
            _videoWebService = (VideoWebServiceFake)_serviceProvider.GetService<IVideoWebService>();
            _notificationApiClient = (NotificationApiClientFake)_serviceProvider.GetService<INotificationApiClient>();
            _userService = (UserServiceFake)_serviceProvider.GetService<IUserService>();

            var logger = new Mock<ILogger<ServiceBusListener>>().Object;
            _serviceBusListener = new ServiceBusListener(
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider),
                new Mock<IServiceBusProcessorWrapper>().Object,
                logger);
        }

        [TearDown]
        public void TearDown()
        {
            _videoApiService.ClearRequests();
            _notificationApiClient.NotificationRequests.Clear();
            _userService.Users.Clear();
            _videoWebService.ClearRequests();
        }

        [SetUp]
        public void SetUp()
        {
            _notificationApiClient.NotificationRequests.Clear();
            _userService.Users.Clear();
        }

        [Test]
        public async Task Should_handle_hearing_ready_for_video_integration_event()
        {
            const string message = @"{
   '$type':'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
   'id':'5d94f88d-68a7-46d4-84d0-b026a452d3c4',
   'timestamp':'2021-02-19T14:50:58.159692Z',
   'integration_event':{
      '$type':'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services',
      'hearing':{
         '$type':'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
         'hearing_id':'a0391117-92e5-41e1-9799-c2cbfc4e9310',
         'scheduled_date_time':'2021-02-19T10:30:00Z',
         'scheduled_duration':45,
         'case_type':'Civil Money Claims',
         'case_number':'01234567890',
         'case_name':'Test Add',
         'hearing_venue_name':'Birmingham Civil and Family Justice Centre',
         'record_audio':true,
         'video_supplier':'Vodafone',
         'conference_room_type': 'VMR'
      },
      'participants':[
         {
            '$type':'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id':'1100ddd1-8cef-48f1-a8ce-5283faff8791',
            'fullname':'Mrs. Automation_Johan Automation_Koch',
            'username':'Automation_eulah.conroy@pagachirthe.info',
            'first_name':'Automation_Johan',
            'last_name':'Automation_Koch',
            'contact_email':'Automation_dale@senger.info',
            'contact_telephone':'TelephoneNumber1',
            'display_name':'Automation_Johan Automation_Koch',
            'hearing_role':'Judge',
            'user_role':'Judge',
            'case_group_type':'judge',
            'representee':'',
            'linked_participants':[
               {
                  '$type':'BookingsApi.Infrastructure.Services.Dtos.LinkedParticipantDto, BookingsApi.Infrastructure.Services',
                  'participant_id':'1100ddd1-8cef-48f1-a8ce-5283faff8791',
                  'linked_id':'a4b2325f-9517-4a7c-a6d0-5dbbe580e371',
                  'type':'interpreter'
               }
            ]
         },
         {
            '$type':'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id':'a4b2325f-9517-4a7c-a6d0-5dbbe580e371',
            'fullname':'Dr. Automation_Hattie Automation_Armstrong',
            'username':'Automation_willis@swiftbednar.name',
            'first_name':'Automation_Hattie',
            'last_name':'Automation_Armstrong',
            'contact_email':'Automation_lavon@brakus.biz',
            'contact_telephone':'TelephoneNumber1',
            'display_name':'Automation_Hattie Automation_Armstrong',
            'hearing_role':'Representative',
            'user_role':'Representative',
            'case_group_type':'claimant',
            'representee':'',
            'linked_participants':[
               {
                  '$type':'BookingsApi.Infrastructure.Services.Dtos.LinkedParticipantDto, BookingsApi.Infrastructure.Services',
                  'participant_id':'a4b2325f-9517-4a7c-a6d0-5dbbe580e371',
                  'linked_id':'1100ddd1-8cef-48f1-a8ce-5283faff8791',
                  'type':'interpreter'
               }
            ]
         }
      ],
      'endpoints':[
         {
            '$type':'BookingsApi.Infrastructure.Services.Dtos.EndpointDto, BookingsApi.Infrastructure.Services',
            'display_name':'display 1',
            'sip':'b5d8c1f5-ef19-4726-b722-3ef86bdfda95',
            'pin':'1234',
            'defence_advocate_username':null,
            'role':'host'
         },
         {
            '$type':'BookingsApi.Infrastructure.Services.Dtos.EndpointDto, BookingsApi.Infrastructure.Services',
            'display_name':'display 2',
            'sip':'72677f04-65d0-41d3-bfe8-845f666c2198',
            'pin':'5678',
            'defence_advocate_username':null,
            'role':'host'
         }
      ]
   }
}";
            await HandleMessage(message);

            _videoApiService.BookNewConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task Should_handle_hearing_cancelled_integration_event()
        {
            const string message = @"{
              '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
              'id': '2e96da15-e99f-4f5c-aaf1-42f853513d63',
              'timestamp': '2019-07-01T14:03:58.0834843Z',
              'integration_event': {
                '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.HearingCancelledIntegrationEvent, Bookings.Infrastructure.Services',
                'hearing_id': 'e33bae78-0cae-4858-a5f0-5134113f1f67',
                'event_type': 'hearingCancelled'
              }
            }";

            await HandleMessage(message);

            _videoApiService.DeleteConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task Should_handle_hearing_details_updated_integration_event()
        {
            const string message = @"{
  '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
  'id': '689a75e1-4f44-470a-9860-9681424e8047',
  'timestamp': '2019-07-02T21:38:59.5195551Z',
  'integration_event': {
    '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.HearingDetailsUpdatedIntegrationEvent, Bookings.Infrastructure.Services',
    'hearing': {
      '$type': 'Bookings.Infrastructure.Services.Dtos.HearingDto, Bookings.Infrastructure.Services',
      'hearing_id': '9e53d84b-cb75-4ff2-a52d-a443960f7430',
      'scheduled_date_time': '2019-07-05T10:45:00Z',
      'scheduled_duration': 100,
      'case_type': 'Generic',
      'case_number': 'CaseNumber',
      'case_name': 'CaseName',
      'record_audio': true
    }
  }
}";
            await HandleMessage(message);
            _videoApiService.UpdateConferenceCount.Should().Be(1);
        }


        [Test]
        public async Task Should_handle_participants_updated_integration_event()
        {
            _videoWebService.PushParticipantsUpdatedMessageCount = 0;

            const string message = @"{
  '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
  'id': 'e9a5bfac-08cc-40e1-ac55-47c2ee44a10a',
  'timestamp': '2023-09-12T09:38:34.445399Z',
  'integration_event': {
    '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingParticipantsUpdatedIntegrationEvent, BookingsApi.Infrastructure.Services',
    'hearing': {
      '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
      'hearing_id': 'c502cb11-1f1c-4865-82e6-37be39cd4ece',
      'group_id': null,
      'scheduled_date_time': '2023-09-12T09:40:00Z',
      'scheduled_duration': 100,
      'case_type': 'Generic',
      'case_number': 'SP ',
      'case_name': 'SP Spike notifications',
      'hearing_venue_name': 'Atlantic Quay Glasgow',
      'record_audio': true,
      'hearing_type': 'Test'
    },
    'existing_participants': [],
    'new_participants': [
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '9df618cf-5110-454d-951d-edc8cf6b3a46',
        'fullname': 'Ms Jane Doe',
        'username': 'jane.doe@hearings.reform.hmcts.net',
        'first_name': 'Jane',
        'last_name': 'Doe',
        'contact_email': 'jane@doe.com',
        'contact_telephone': '0123456789',
        'display_name': 'Ms Jane Doe',
        'hearing_role': 'Witness',
        'user_role': 'Individual',
        'case_group_type': 'applicant',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': false
      }
    ],
    'removed_participants': [],
    'linked_participants': []
  }
}";

            await HandleMessage(message);
            _videoApiService.UpdateConferenceParticipantsAsyncCount.Should().Be(1);
            _videoWebService.PushParticipantsUpdatedMessageCount.Should().Be(1);
        }

        [Test]
        public async Task Should_handle_participant_removed_integration_event()
        {
            const string message = @"{
  '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
  'id': '9e4bb2b7-3187-419c-a7c8-b1e17a3cbb6f',
  'timestamp': '2019-07-02T21:48:08.8808044Z',
  'integration_event': {
    '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.ParticipantRemovedIntegrationEvent, Bookings.Infrastructure.Services',
    'hearing_id': '015a0b0e-a16d-4076-a2b2-328b1d26212b',
    'participant_id': 'ea801426-0ea2-4eab-aaf0-647ae146397a'
  }
}";

            _videoApiService.InitConferenceResponse();
            _videoApiService.ConferenceResponse.Id = Guid.Parse("9e4bb2b7-3187-419c-a7c8-b1e17a3cbb6f");
            _videoApiService.ConferenceResponse.Participants[0].RefId =
                Guid.Parse("ea801426-0ea2-4eab-aaf0-647ae146397a");

            await HandleMessage(message);
            _videoApiService.RemoveParticipantFromConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task Should_handle_participant_updated_integration_event()
        {
            const string message = @"{
  '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
  'id': 'ab013e39-d159-4836-848e-034d2ebbe37a',
  'timestamp': '2019-07-02T21:57:57.7904475Z',
  'integration_event': {
    '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.ParticipantUpdatedIntegrationEvent, Bookings.Infrastructure.Services',
    'hearing_id': '769d17f6-85f1-4624-bc07-ffdac8ddb619',
    'participant': {
      '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
      'participant_id': 'af9afb87-5cf8-4813-b3dc-0ea96f77e752',
      'fullname': 'Mr. Garnet Bosco',
      'first_name': 'FirstName1',
      'last_name': 'LastName1',
                    'contact_email': 'tst@hmcts.net',
                    'contact_telephone': '01234567890',
      'username': 'pinkie_kuhlman@hmcts.net',
      'display_name': 'Raegan Pollich V',
      'hearing_role': 'Solicitor',
      'user_role': 'Representative',
      'case_group_type': 'Respondent',
      'representee': 'Bobby Upton'
    }
  }
}";
            _videoApiService.InitConferenceResponse();
            _videoApiService.ConferenceResponse.Id = Guid.Parse("ab013e39-d159-4836-848e-034d2ebbe37a");
            _videoApiService.ConferenceResponse.Participants[0].RefId =
                Guid.Parse("af9afb87-5cf8-4813-b3dc-0ea96f77e752");

            await HandleMessage(message);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _userService.UpdateUserAccountCount.Should().Be(1);
        }

        [Test]
        public void Should_throw_exception_when_message_cannot_be_parsed()
        {
            const string message = @"
          {
            'id': 'ab013e39-d159-4836-848e-034d2ebbe37a',
            'timestamp': '2019-07-02T21:57:57.7904475Z',
            'integration_event': {
              'hearing_id': '769d17f6-85f1-4624-bc07-ffdac8ddb619',
              'participant': {
                'participant_id': 'af9afb87-5cf8-4813-b3dc-0ea96f77e752',
                'fullname': 'Mr. Garnet Bosco',
                'first_name': 'FirstName1',
                'last_name': 'LastName1',
                'username': 'pinkie_kuhlman@hmcts.net',
                'display_name': 'Raegan Pollich V',
                'hearing_role': 'Solicitor',
                'user_role': 'Representative',
                'case_group_type': 'Respondent',
                'representee': 'Bobby Upton'
              }
            }
          }";
            Func<Task> f = async () => { await HandleMessage(message); };
            f.Should().ThrowAsync<JsonSerializationException>();
        }

        [Test]
        public async Task Should_handle_hearing_create_and_notify_user_integration_event()
        {
            _videoWebService.PushParticipantsUpdatedMessageCount = 0;

            const string message = @"{
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'c4286e6a-d4e8-45f9-a992-068813990f8c',
              'timestamp': '2023-11-18T22:45:25.7936899Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.CreateAndNotifyUserIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': '4dc310a9-0716-4dba-bfb8-a221936a4d3d',
                  'scheduled_date_time': '2023-11-19T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participnat_id': 'a259a318-8fcc-4820-aff4-be098c41be6c',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_1486595270@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_1829854172@hmcts.net',
                  'representee': ''
                }
              }
            }";
            await HandleMessage(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _videoWebService.PushParticipantsUpdatedMessageCount.Should().Be(1);

            var notificationRequest = (SignInDetailsEmailRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.ContactEmail.Should().Be("Automation_1486595270@hmcts.net");
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.Username.Should().Be("Automation_FirstName.Automation_LastName");
            notificationRequest.Password.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Should_handle_hearing_amendment_integration_event()
        {
            const string message = @"{
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'aaf7f048-faf5-4d40-b5b2-8afce08fbe9a',
              'timestamp': '2023-11-18T23:37:04.670075Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingAmendmentNotificationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': '91c592d8-5ff8-4774-b456-87d7b4e319f3',
                  'scheduled_date_time': '2023-11-19T11:45:00Z',
                  'case_name': 'Case name',
                  'case_number': 'Original Hearing',
                  'participant_id': '73f2053e-74f1-4d6c-b817-246f4b22e665',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_226153990@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_338564597@hmcts.net',
                  'representee': ''
                },
                'new_scheduled_date_time': '2023-11-20T00:00:00Z'
              }
            }";
            await HandleMessage(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);
            var notificationRequest = (HearingAmendmentRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be(new Guid("91c592d8-5ff8-4774-b456-87d7b4e319f3"));
            notificationRequest.ContactEmail.Should().Be("Automation_226153990@hmcts.net");
            notificationRequest.ParticipantId.Should().Be(new Guid("73f2053e-74f1-4d6c-b817-246f4b22e665"));
            notificationRequest.CaseName.Should().Be("Case name");
            notificationRequest.PreviousScheduledDateTime.Should()
                .Be(DateTime.Parse($"2023-11-19T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.NewScheduledDateTime.Should()
                .Be(DateTime.Parse($"2023-11-20T00:00:00Z", CultureInfo.InvariantCulture));
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.CaseNumber.Should().Be("Original Hearing");
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.DisplayName.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.Representee.Should().Be("");
            notificationRequest.Username.Should().Be("Automation_338564597@hmcts.net");
        }

        [Test]
        public async Task Should_handle_multiday_hearing_integration_event()
        {
            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'e5c7fe37-15f6-4b76-b336-7cd02db7d34a',
              'timestamp': '2023-11-18T23:25:22.2296966Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.MultiDayHearingIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': '1a77b12f-becc-43fa-891c-15e882d2f37b',
                  'scheduled_date_time': '2023-11-19T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participant_id': '314cfdcc-af6a-443f-a89a-4dbdc652aa1b',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_556303923@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_840074883@hmcts.net',
                  'representee': ''
                },
                'total_days': 2
              }
            }";

            await HandleMessage(message);
            _notificationApiClient.NotificationRequests.Count.Should().Be(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);

            var notificationRequest =
                (ExistingUserMultiDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.CaseNumber.Should().Be("AutoTest");
            notificationRequest.ContactEmail.Should().Be("Automation_556303923@hmcts.net");
            notificationRequest.DisplayName.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.HearingId.Should().Be(new Guid("1a77b12f-becc-43fa-891c-15e882d2f37b"));
            notificationRequest.ParticipantId.Should().Be(new Guid("314cfdcc-af6a-443f-a89a-4dbdc652aa1b"));
            notificationRequest.Representee.Should().Be(string.Empty);
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-19T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.TotalDays.Should().Be(2);
            notificationRequest.Username.Should().Be("Automation_840074883@hmcts.net");
        }

        [Test]
        public async Task Should_handle_hearing_notification_integration_event()
        {
            const string message = @"{
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'aaf7f048-faf5-4d40-b5b2-8afce08fbe9a',
              'timestamp': '2023-11-18T23:37:04.670075Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingNotificationIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': '91c592d8-5ff8-4774-b456-87d7b4e319f3',
                  'scheduled_date_time': '2023-11-19T11:45:00Z',
                  'case_name': 'Case name',
                  'case_number': 'Original Hearing',
                  'participant_id': '73f2053e-74f1-4d6c-b817-246f4b22e665',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_226153990@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_338564597@hmcts.net',
                  'representee': ''
                }
              }
            }";

            await HandleMessage(message);
            _notificationApiClient.NotificationRequests.Count.Should().Be(1);

            var notificationRequest =
                (ExistingUserSingleDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be("91c592d8-5ff8-4774-b456-87d7b4e319f3");
            notificationRequest.ContactEmail.Should().Be("Automation_226153990@hmcts.net");
            notificationRequest.ParticipantId.Should().Be("73f2053e-74f1-4d6c-b817-246f4b22e665");
            notificationRequest.CaseName.Should().Be("Case name");
            notificationRequest.DisplayName.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.Representee.Should().Be(string.Empty);
            notificationRequest.Username.Should().Be("Automation_338564597@hmcts.net");
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-19T11:45:00Z", CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task Should_create_user_account_and_send_non_ejud_hearing_notifications_for_eJudUsername()
        {
            const string message = @" {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
            'id': '1e2665c4-a4a0-4076-863d-5a2f1bf7e6dc',
            'timestamp': '2022-08-23T16:03:53.8526422Z',
            'integration_event': {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services',
            'hearing': {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
            'hearing_id': '81d791e9-e3e6-4d54-a0cb-d762579b1409',
            'group_id': null,
            'scheduled_date_time': '2022-08-23T17:00:00Z',
            'scheduled_duration': 45,
            'case_type': 'Family',
            'case_number': 'VIH9276--EJUDGE-PM',
            'case_name': 'VIH9276--EJUDGE-PM',
            'hearing_venue_name': 'Skipton County Court and Family Court',
            'record_audio': true,
            'hearing_type': 'Family Private Law'
            },
            'participants': [
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '536dda9f-e0f3-4293-8cb9-1ac0aba8c435',
            'fullname': ' Manual Judge_06',
            'username': 'manual.judge_06@judiciarystaging.onmicrosoft.com',
            'first_name': 'Manual',
            'last_name': 'Judge_06',
            'contact_email': 'manual.judge_06@judiciarystaging.onmicrosoft.com',
            'contact_telephone': null,
            'display_name': 'Manual Judge_06',
            'hearing_role': 'Judge',
            'user_role': 'Judge',
            'case_group_type': 'judge',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': '',
            'contact_phone_for_non_e_jud_judge_user': '',
            'send_hearing_notification_if_new': true
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '5e4768d9-e949-4f7b-a512-c257911f6584',
            'fullname': 'Ms Manual 7 Panel 7',
            'username': 'manual_panel_7@judiciarystaging.onmicrosoft.com',
            'first_name': 'Manual 7',
            'last_name': 'Panel 7',
            'contact_email': 'manual_panel_7@judiciarystaging.onmicrosoft.com',
            'contact_telephone': '7654',
            'display_name': 'PM',
            'hearing_role': 'Panel Member',
            'user_role': 'Judicial Office Holder',
            'case_group_type': 'panelMember',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null,
            'send_hearing_notification_if_new': true
            }
            ],
            'endpoints': []
            }
            }";

            await HandleMessage(message);

            _userService.Users.Should().HaveCount(0);
            _videoApiService.BookNewConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task Should_send_welcome_notifications_for_new_participant()
        {
            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': '22c2627c-d93e-4084-8351-20358f724010',
              'timestamp': '2023-11-01T22:07:14.7852801Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.NewParticipantWelcomeEmailEvent, BookingsApi.Infrastructure.Services',
                'welcome_email': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.WelcomeEmailDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': 'faf75f2e-8e40-424a-8b30-767b57e9477e',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participnat_id': '6b4f565c-2eb2-404c-86e3-a00389657845',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'contact_email': 'Automation_129055124@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Representative'
                }
              }
            }";

            await HandleMessage(message);

            _notificationApiClient.NotificationRequests.Should().HaveCount(1);

            var notificationRequest = (NewUserWelcomeEmailRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.CaseNumber.Should().Be("AutoTest");
            notificationRequest.ContactEmail.Should().Be("Automation_129055124@hmcts.net");
            notificationRequest.HearingId.Should().Be(new Guid("faf75f2e-8e40-424a-8b30-767b57e9477e"));
            notificationRequest.ParticipantId.Should().Be(new Guid("6b4f565c-2eb2-404c-86e3-a00389657845"));
            notificationRequest.RoleName.Should().Be("Representative");
        }

        [Test]
        public async Task Should_create_user_send_confirmation_notifications_for_new_participant()
        {
            _videoWebService.PushParticipantsUpdatedMessageCount = 0;

            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': '004ad2e6-e643-4696-9c4a-34ad7c74cb85',
              'timestamp': '2023-11-01T22:10:06.4925888Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.NewParticipantHearingConfirmationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': 'faf75f2e-8e40-424a-8b30-767b57e9477e',
                  'scheduled_date_time': '2023-11-02T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participant_id': '702dd1c2-c8f4-40b4-a096-5b77796c2dcd',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_1316542910@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_1680416252@hmcts.net',
                  'representee': ''
                }
              }
            }";


            await HandleMessage(message);

            _userService.Users.Should().HaveCount(1);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);

            var notificationRequest =
                (NewUserSingleDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be(new Guid("faf75f2e-8e40-424a-8b30-767b57e9477e"));
            notificationRequest.ContactEmail.Should().Be("Automation_1316542910@hmcts.net");
            notificationRequest.ParticipantId.Should().Be(new Guid("702dd1c2-c8f4-40b4-a096-5b77796c2dcd"));
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-02T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.Username.Should().NotBeNullOrEmpty();
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.CaseNumber.Should().Be("AutoTest");
            notificationRequest.RandomPassword.Should().NotBeNullOrEmpty();
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");

            _videoWebService.PushParticipantsUpdatedMessageCount.Should().Be(1);
        }

        [Test]
        public async Task Should_send_confirmation_notifications_for_existing_participant()
        {
            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'ad43b989-4ffa-4817-9175-4afe1c559d4c',
              'timestamp': '2023-11-01T22:11:15.5866489Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.ExistingParticipantHearingConfirmationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': 'faf75f2e-8e40-424a-8b30-767b57e9477e',
                  'scheduled_date_time': '2023-11-02T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participant_id': '246ab9d2-993b-4c7d-9e4c-ec6246d5a9a5',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_54514578@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_311869852@hmcts.net',
                  'representee': ''
                }
              }
            }";

            await HandleMessage(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);

            var notificationRequest =
                (ExistingUserSingleDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be("faf75f2e-8e40-424a-8b30-767b57e9477e");
            notificationRequest.ContactEmail.Should().Be("Automation_54514578@hmcts.net");
            notificationRequest.ParticipantId.Should().Be("246ab9d2-993b-4c7d-9e4c-ec6246d5a9a5");
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.DisplayName.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.Representee.Should().Be(string.Empty);
            notificationRequest.Username.Should().Be("Automation_311869852@hmcts.net");
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-02T11:45:00Z", CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task Should_send_multiday_confirmation_notifications_for_new_participant()
        {
            _videoWebService.PushParticipantsUpdatedMessageCount = 0;

            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': 'eedf8d6d-901d-44f3-ba41-544ebf43c610',
              'timestamp': '2023-11-01T22:12:22.1124871Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.NewParticipantMultidayHearingConfirmationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': 'fa9edabe-6d48-48df-b5c4-1a43caad8e6f',
                  'scheduled_date_time': '2023-11-02T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participant_id': '43065898-c18f-4f42-b39f-dc56b4906445',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_1007248621@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Representative',
                  'username': 'Automation_15661803@hmcts.net',
                  'representee': ''
                },
                'total_days': 2
              }
            }";

            await HandleMessage(message);
            _userService.Users.Should().HaveCount(1);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);

            var notificationRequest =
                (NewUserMultiDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be(Guid.Parse("fa9edabe-6d48-48df-b5c4-1a43caad8e6f"));
            notificationRequest.ContactEmail.Should().Be("Automation_1007248621@hmcts.net");
            notificationRequest.ParticipantId.Should().Be(new Guid("43065898-c18f-4f42-b39f-dc56b4906445"));
            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.CaseNumber.Should().Be("AutoTest");
            notificationRequest.RandomPassword.Should().NotBeNullOrEmpty();
            notificationRequest.RoleName.Should().Be("Representative");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-02T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.TotalDays.Should().Be(2);
            notificationRequest.Username.Should().NotBeNullOrEmpty();

            _videoWebService.PushParticipantsUpdatedMessageCount.Should().Be(1);
        }

        [Test]
        public async Task Should_send_multiday_confirmation_notifications_for_existing_participant()
        {
            const string message = @" {
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': '64d759f7-3b2f-491a-a3b3-17d2b22f3ab8',
              'timestamp': '2023-11-01T22:13:18.8512123Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.ExistingParticipantMultidayHearingConfirmationEvent, BookingsApi.Infrastructure.Services',
                'hearing_confirmation_for_participant': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingConfirmationForParticipantDto, BookingsApi.Infrastructure.Services',
                  'hearing_id': 'fa9edabe-6d48-48df-b5c4-1a43caad8e6f',
                  'scheduled_date_time': '2023-11-02T11:45:00Z',
                  'case_name': 'Test',
                  'case_number': 'AutoTest',
                  'participant_id': '79cef9a1-c040-45da-8f9b-891583be1b59',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_1172867501@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_1125238517@hmcts.net',
                  'representee': ''
                },
                'total_days': 2
              }
            }";

            await HandleMessage(message);

            _notificationApiClient.NotificationRequests.Should().HaveCount(1);

            var notificationRequest =
                (ExistingUserMultiDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.Name.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.CaseName.Should().Be("Test");
            notificationRequest.CaseNumber.Should().Be("AutoTest");
            notificationRequest.ContactEmail.Should().Be("Automation_1172867501@hmcts.net");
            notificationRequest.DisplayName.Should().Be("Automation_FirstName Automation_LastName");
            notificationRequest.HearingId.Should().Be(new Guid("fa9edabe-6d48-48df-b5c4-1a43caad8e6f"));
            notificationRequest.ParticipantId.Should().Be(new Guid("79cef9a1-c040-45da-8f9b-891583be1b59"));
            notificationRequest.Representee.Should().Be(string.Empty);
            notificationRequest.RoleName.Should().Be("Individual");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2023-11-02T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.TotalDays.Should().Be(2);
            notificationRequest.Username.Should().Be("Automation_1125238517@hmcts.net");
        }

        [Test]
        public async Task should_handle_EndpointUpdatedIntegrationEvent()
        {
            const string message = @"
                                 {
                                   '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
                                    'id': '20579099-f03b-4ace-adf5-fb6d4ce5760f',
                                    'timestamp': '2024-10-18T13:38:05.886327Z',
                                    'integration_event': {
                                       '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.EndpointUpdatedIntegrationEvent, BookingsApi.Infrastructure.Services',
                                       'hearing_id': 'fb1ab55d-80ce-4700-91a3-8136e3a844d6',
                                       'sip': '5622555833@dev.vh-hmcts.co.uk',
                                       'display_name': 'Endpoint 1',
                                       'defence_advocate': null,
                                       'role': 'host'
                                    }
                                 }
                                 ";

            await HandleMessage(message);
            _videoApiService.UpdateEndpointInConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task should_handle_HearingsAllocatedIntegrationEvent()
        {
            const string message = @"{
              '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
              'id': '4b37338c-0507-4ee7-b5f5-d56172d5078b',
              'timestamp': '2025-01-29T09:05:47.070304Z',
              'integration_event': {
                '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingsAllocatedIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearings': [
                  {
                    '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
                    'hearing_id': '8c89895d-bc71-47a3-9777-0d26270b07a2',
                    'group_id': null,
                    'scheduled_date_time': '2025-01-29T10:30:00Z',
                    'scheduled_duration': 45,
                    'case_type': 'Financial Remedy',
                    'case_number': 'TestSearchQueryInt',
                    'case_name': 'Integration',
                    'hearing_venue_name': 'Birmingham Civil and Family Justice Centre',
                    'record_audio': false,
                    'case_type_service_id': 'ABA2',
                    'video_supplier': 'vodafone',
                    'conference_room_type': 'vmr',
                    'is_venue_welsh': false
                  }
                ],
                'allocated_cso': {
                  '$type': 'BookingsApi.Infrastructure.Services.Dtos.JusticeUserDto, BookingsApi.Infrastructure.Services',
                  'user_id': '74c3a02c-672b-4b89-81fa-2b9c7c39d13a',
                  'username': '924abca9a76147c599da348c14bda780@test.com',
                  'full_name': 'CSO Name',
                  'user_roles': []
                }
              }
            }";

            await HandleMessage(message);
            _videoWebService.PushAllocationToCsoUpdatedMessageCount.Should().Be(1);
        }

        [Test]
        public async Task Should_handle_JudgeUpdatedIntegrationEvent()
        {
            const string message = @"{
             '$type':'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
             'id':'5d94f88d-68a7-46d4-84d0-b026a452d3c4',
             'timestamp':'2021-02-19T14:50:58.159692Z',
             'integration_event':{
                '$type':'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.JudgeUpdatedIntegrationEvent, BookingsApi.Infrastructure.Services',
                'hearing':{
                   '$type':'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
                   'hearing_id':'a0391117-92e5-41e1-9799-c2cbfc4e9310',
                   'scheduled_date_time':'2021-02-19T10:30:00Z',
                   'scheduled_duration':45,
                   'case_type':'Civil Money Claims',
                   'case_number':'01234567890',
                   'case_name':'Test Add',
                   'hearing_venue_name':'Birmingham Civil and Family Justice Centre',
                   'record_audio':true,
                   'video_supplier':'Vodafone',
                   'conference_room_type': 'VMR'
                },
                'judge':{
                    '$type':'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
                    'participant_id':'1100ddd1-8cef-48f1-a8ce-5283faff8791',
                    'fullname':'Mrs. Automation_Johan Automation_Koch',
                    'username':'Automation_eulah.conroy@pagachirthe.info',
                    'first_name':'Automation_Johan',
                    'last_name':'Automation_Koch',
                    'contact_email':'contact-email@email.com',
                    'contact_telephone':'TelephoneNumber1',
                    'display_name':'Automation_Johan Automation_Koch',
                    'hearing_role':'Judge',
                    'user_role':'Judge',
                    'representee':'',
                    'linked_participants':[]
                },
                'send_notification': true
             }
          }";

            _videoApiService.ConferenceResponse = new ConferenceDetailsResponse
            {
                Id = Guid.NewGuid(),
                HearingId = new Guid("a0391117-92e5-41e1-9799-c2cbfc4e9310"),
                Participants = new List<ParticipantResponse>
                {
                    new()
                    {
                        RefId = new Guid("1100ddd1-8cef-48f1-a8ce-5283faff8791"),
                        ContactEmail = "Automation_dale@senger.info"
                    }
                }
            };

            await HandleMessage(message);

            var notificationRequest =
                (ExistingUserSingleDayHearingConfirmationRequest)_notificationApiClient.NotificationRequests[0];

            notificationRequest.HearingId.Should().Be("a0391117-92e5-41e1-9799-c2cbfc4e9310");
            notificationRequest.ContactEmail.Should().Be("contact-email@email.com");
            notificationRequest.ParticipantId.Should().Be("1100ddd1-8cef-48f1-a8ce-5283faff8791");
            notificationRequest.CaseName.Should().Be("Test Add");
            notificationRequest.DisplayName.Should().Be("Automation_Johan Automation_Koch");
            notificationRequest.Name.Should().Be("Automation_Johan Automation_Koch");
            notificationRequest.Representee.Should().BeNull();
            notificationRequest.Username.Should().Be("Automation_eulah.conroy@pagachirthe.info");
            notificationRequest.RoleName.Should().Be("Judge");
            notificationRequest.ScheduledDateTime.Should()
                .Be(DateTime.Parse("2021-02-19T10:30:00Z", CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task should_process_a_single_day_hearing_ready_event_with_a_judge_only()
        {
            const string message = @"{
  '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
  'id': 'e0bbb9ed-ce49-4e69-94e7-3e35e7010206',
  'timestamp': '2023-09-15T09:03:50.889496Z',
  'integration_event': {
    '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services',
    'hearing': {
      '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
      'hearing_id': 'e2ef8a71-6d22-486b-8876-a69aceac86d7',
      'group_id': null,
      'scheduled_date_time': '2023-09-15T09:08:46.636188Z',
      'scheduled_duration': 5,
      'case_type': 'Civil Money Claims',
      'case_number': '6918/2815',
      'case_name': 'Bookings Api Integration Automated 9532050',
      'hearing_venue_name': 'Birmingham Civil and Family Justice Centre',
      'record_audio': true,
      'hearing_type': 'First Application'
    },
    'participants': [
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': 'c20b90b2-8fb1-4e65-b77b-fd381821ccad',
        'fullname': 'Mrs Automation_Judge Judge_1',
        'username': 'automation_judge_judge_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Judge',
        'last_name': 'Judge_1',
        'contact_email': 'automation_judge_judge_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Judge Judge_1',
        'hearing_role': 'Judge',
        'user_role': 'Judge',
        'case_group_type': 'judge',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': '',
        'contact_phone_for_non_e_jud_judge_user': '',
        'send_hearing_notification_if_new': true
      }
    ],
    'endpoints': []
  }
}";
            await HandleMessage(message);

            _videoApiService.BookNewConferenceCount.Should().Be(1);
            _videoWebService.PushNewConferenceAddedMessageCount.Should().Be(1);
        }

        [Test]
        public async Task should_process_a_single_day_hearing_ready_event_with_a_judge_and_participants()
        {
            const string message = @"{
  '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
  'id': '25839fbd-d19a-4ff8-908d-1c844b9171bc',
  'timestamp': '2023-09-15T09:17:22.211731Z',
  'integration_event': {
    '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services',
    'hearing': {
      '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
      'hearing_id': '3946edba-933e-49cb-a328-e350814b6fa2',
      'group_id': null,
      'scheduled_date_time': '2023-09-15T09:22:18.10116Z',
      'scheduled_duration': 5,
      'case_type': 'Civil Money Claims',
      'case_number': '6918/2815',
      'case_name': 'Bookings Api Integration Automated 9532050',
      'hearing_venue_name': 'Birmingham Civil and Family Justice Centre',
      'record_audio': true,
      'hearing_type': 'First Application'
    },
    'participants': [
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': 'c0102934-9c49-4cea-bcfc-45fbf503f8a0',
        'fullname': 'Mrs Automation_Respondent LitigantInPerson_1',
        'username': 'automation_respondent_litigantinperson_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Respondent',
        'last_name': 'LitigantInPerson_1',
        'contact_email': 'automation_respondent_litigantinperson_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Respondent LitigantInPerson_1',
        'hearing_role': 'Litigant in person',
        'user_role': 'Individual',
        'case_group_type': 'respondent',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '04850798-4b69-4d86-8fb7-5bafda64703d',
        'fullname': 'Mrs Automation_Respondent Representative_1',
        'username': 'automation_respondent_representative_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Respondent',
        'last_name': 'Representative_1',
        'contact_email': 'automation_respondent_representative_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Respondent Representative_1',
        'hearing_role': 'Representative',
        'user_role': 'Representative',
        'case_group_type': 'respondent',
        'representee': 'Automation_Respondent LitigantInPerson_1',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': true
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '9e30d442-53ca-458b-90d8-9588cee1ddde',
        'fullname': 'Mrs Automation_Judge Judge_1',
        'username': 'automation_judge_judge_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Judge',
        'last_name': 'Judge_1',
        'contact_email': 'automation_judge_judge_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Judge Judge_1',
        'hearing_role': 'Judge',
        'user_role': 'Judge',
        'case_group_type': 'judge',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': '',
        'contact_phone_for_non_e_jud_judge_user': '',
        'send_hearing_notification_if_new': true
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': 'd6275f3c-5b21-4362-a0ca-d8319a68492b',
        'fullname': 'Mrs Automation_Applicant LitigantInPerson_1',
        'username': 'automation_applicant_litigantinperson_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Applicant',
        'last_name': 'LitigantInPerson_1',
        'contact_email': 'automation_applicant_litigantinperson_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Applicant LitigantInPerson_1',
        'hearing_role': 'Litigant in person',
        'user_role': 'Individual',
        'case_group_type': 'applicant',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': true
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': 'c9ca299c-c83d-4a1b-bef9-efa8fbb1d57f',
        'fullname': 'Mrs Automation_Applicant Representative_1',
        'username': 'automation_applicant_representative_1@hearings.reform.hmcts.net',
        'first_name': 'Automation_Applicant',
        'last_name': 'Representative_1',
        'contact_email': 'automation_applicant_representative_1@hmcts.net',
        'contact_telephone': '01234567890',
        'display_name': 'Automation_Applicant Representative_1',
        'hearing_role': 'Representative',
        'user_role': 'Representative',
        'case_group_type': 'applicant',
        'representee': 'Automation_Applicant LitigantInPerson_1',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': true
      }
    ],
    'endpoints': []
  }
}";
            await HandleMessage(message);

            _videoApiService.BookNewConferenceCount.Should().Be(1);
            _videoWebService.PushNewConferenceAddedMessageCount.Should().Be(1);
        }

        private async Task HandleMessage(string messageContent)
        {
            var serviceBusMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
                body: BinaryData.FromString(messageContent)
            );

            var argsMock = new Mock<ProcessMessageEventArgs>(
                serviceBusMessage,
                Mock.Of<ServiceBusReceiver>(),
                null!
            );

            await _serviceBusListener.HandleMessage(argsMock.Object);
        }
    }
}