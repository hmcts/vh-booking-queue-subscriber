﻿using System.Globalization;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.UserApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunctionTests
{
    public class RunTests : MessageHandlerTestBase
    {
        private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
        private VideoApiServiceFake _videoApiService;
        private VideoWebServiceFake _videoWebService;
        private NotificationApiClientFake _notificationApiClient;
        private UserServiceFake _userService;
        private BookingQueueSubscriberFunction _sut;
        private ILogger<BookingQueueSubscriberFunction> _logger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _logger = new Mock<ILogger<BookingQueueSubscriberFunction>>().Object;
            _videoApiService = (VideoApiServiceFake) _serviceProvider.GetService<IVideoApiService>();
            _videoWebService = (VideoWebServiceFake) _serviceProvider.GetService<IVideoWebService>();
            _notificationApiClient = (NotificationApiClientFake)_serviceProvider.GetService<INotificationApiClient>();
            _userService = (UserServiceFake)_serviceProvider.GetService<IUserService>();
            _sut = new BookingQueueSubscriberFunction(new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider), _logger);
        }

        [TearDown]
        public void TearDown()
        {
          _videoApiService.ClearRequests();
            _notificationApiClient.NotificationRequests.Clear();
            _userService.Users.Clear();
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
            await _sut.Run(message);

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

            await _sut.Run(message);
            
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
            await _sut.Run(message);
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
            
            await _sut.Run(message);
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
            _videoApiService.ConferenceResponse.Participants[0].RefId = Guid.Parse("ea801426-0ea2-4eab-aaf0-647ae146397a");
            
            await _sut.Run(message);
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
            _videoApiService.ConferenceResponse.Participants[0].RefId = Guid.Parse("af9afb87-5cf8-4813-b3dc-0ea96f77e752");
            
            await _sut.Run(message);
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
          Func<Task> f = async () => { await _sut.Run(message); };
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
            await _sut.Run(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _videoWebService.PushParticipantsUpdatedMessageCount.Should().Be(1);
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
            await _sut.Run(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);
            var notificationRequest = (HearingAmendmentRequest)_notificationApiClient.NotificationRequests[0];
            
            notificationRequest.HearingId.Should().Be(new Guid("91c592d8-5ff8-4774-b456-87d7b4e319f3"));
            notificationRequest.ContactEmail.Should().Be("Automation_226153990@hmcts.net");
            notificationRequest.ParticipantId.Should().Be(new Guid("73f2053e-74f1-4d6c-b817-246f4b22e665"));
            notificationRequest.CaseName.Should().Be("Case name");
            notificationRequest.PreviousScheduledDateTime.Should().Be(DateTime.Parse($"2023-11-19T11:45:00Z", CultureInfo.InvariantCulture));
            notificationRequest.NewScheduledDateTime.Should().Be(DateTime.Parse($"2023-11-20T00:00:00Z", CultureInfo.InvariantCulture));
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
                  'participnat_id': '314cfdcc-af6a-443f-a89a-4dbdc652aa1b',
                  'first_name': 'Automation_FirstName',
                  'last_name': 'Automation_LastName',
                  'display_name': 'Automation_FirstName Automation_LastName',
                  'contact_email': 'Automation_556303923@hmcts.net',
                  'contact_telephone': '01234567890',
                  'user_role': 'Individual',
                  'username': 'Automation_840074883@hmcts.net',
                  'representee': ''
                }
              }
            }";

            await _sut.Run(message);
            _notificationApiClient.NotificationRequests.Count.Should().Be(1);
            _videoApiService.BookNewConferenceCount.Should().Be(0);
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

            await _sut.Run(message);

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

            await _sut.Run(message);

            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
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
                  'participnat_id': '702dd1c2-c8f4-40b4-a096-5b77796c2dcd',
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

          
            
            await _sut.Run(message);
            
            _userService.Users.Should().HaveCount(1);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
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
                  'participnat_id': '246ab9d2-993b-4c7d-9e4c-ec6246d5a9a5',
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

            await _sut.Run(message);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
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
                  'participnat_id': '43065898-c18f-4f42-b39f-dc56b4906445',
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

            await _sut.Run(message);
            _userService.Users.Should().HaveCount(1);
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
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
                  'participnat_id': '79cef9a1-c040-45da-8f9b-891583be1b59',
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

            await _sut.Run(message);

            _notificationApiClient.NotificationRequests.Should().HaveCount(1);
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
          
          await _sut.Run(message);
          _videoApiService.UpdateEndpointInConferenceCount.Should().Be(1);
          
        }
    }
}