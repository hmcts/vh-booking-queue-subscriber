using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunctionTests
{
    public class RunTests : MessageHandlerTestBase
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

            _sut = new BookingQueueSubscriberFunction(new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
        }

        [TearDown]
        public void TearDown()
        {
          _videoApiService.ClearRequests();
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
         'record_audio':true
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
            'defence_advocate_username':null
         },
         {
            '$type':'BookingsApi.Infrastructure.Services.Dtos.EndpointDto, BookingsApi.Infrastructure.Services',
            'display_name':'display 2',
            'sip':'72677f04-65d0-41d3-bfe8-845f666c2198',
            'pin':'5678',
            'defence_advocate_username':null
         }
      ]
   }
}";
            await _sut.Run(message, new LoggerFake());

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

            await _sut.Run(message, new LoggerFake());
            
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
            await _sut.Run(message, new LoggerFake());
            _videoApiService.UpdateConferenceCount.Should().Be(1);
        }


        [Test]
        public async Task Should_handle_participants_updated_integration_event()
        {
            _videoWebService.PushParticipantsUpdatedMessageCount = 0;

            const string message = @"{
'$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
'id': 'd0089d35-7a49-4fa3-8414-fb3c77186bfc',
'timestamp': '2022-06-16T09:45:35.5875542Z',
'integration_event': {
'$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.ParticipantsAddedIntegrationEvent, BookingsApi.Infrastructure.Services',
'hearing': {
'$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
'hearing_id': 'fe5bcb33-f950-4b64-978b-1d130280b154',
'group_id': null,
'scheduled_date_time': '2022-07-12T09:49:56.924Z',
'scheduled_duration': 45,
'case_type': 'Civil Money Claims',
'case_number': '123333',
'case_name': 'Rambo5 vs terminator5',
'hearing_venue_name': 'Aberdeen Tribunal Hearing Centre',
'record_audio': false
},
'participants': [
{
'$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
'participant_id': '96c04fca-b3d0-4526-9120-7916b55a4657',
'fullname': 'Mr Partcipant Eleven',
'username': 'Partcipant.Eleven@hearings.reform.hmcts.net',
'first_name': 'Partcipant',
'last_name': 'Eleven',
'contact_email': 'Part.eleven@gmail.com',
'contact_telephone': '1213131321',
'display_name': 'One',
'hearing_role': 'Litigant in person',
'user_role': 'Individual',
'case_group_type': 'claimant',
'representee': '',
'linked_participants': [],
'contact_email_for_non_e_jud_judge_user': ''
}
]
}
}";
            
            await _sut.Run(message, new LoggerFake());
            _videoApiService.AddParticipantsToConferenceCount.Should().Be(1);
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
            
            await _sut.Run(message, new LoggerFake());
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
            
            await _sut.Run(message, new LoggerFake());
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
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
          var logger = new LoggerFake();
          const string errorMessageMatch = "Unable to deserialize into EventMessage";
          Func<Task> f = async () => { await _sut.Run(message, logger); };
          f.Should().ThrowAsync<Exception>().WithMessage(errorMessageMatch);
          logger.Messages.Should().ContainMatch($"{errorMessageMatch}*");
        }

        [Test]
        public async Task Should_handle_hearing_create_and_notify_user_integration_event()
        {
            const string message = @"{
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
            'id': '0473e722-a7e1-4af2-b76e-42332d988a4d',
            'timestamp': '2022-06-24T16:04:11.3750446Z',
            'integration_event': {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.CreateAndNotifyUserIntegrationEvent, BookingsApi.Infrastructure.Services',
            'hearing': {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
            'hearing_id': '80b79a4e-a104-46ac-b31c-c20edc2c5c8a',
            'group_id': null,
            'scheduled_date_time': '2022-07-19T09:49:56.924Z',
            'scheduled_duration': 45,
            'case_type': 'Civil Money Claims',
            'case_number': '54453434',
            'case_name': 'Rambo4 vs terminator4',
            'hearing_venue_name': 'Aberdeen Tribunal Hearing Centre',
            'record_audio': false
            },
            'participants': [
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '6706c2f5-0698-4e8d-9034-9e148e9b8bf2',
            'fullname': 'Mr Brew milkyOne',
            'username': 'brewmilkyOne@gmail.com',
            'first_name': 'Brew',
            'last_name': 'milkyOne',
            'contact_email': 'brewmilkyOne@gmail.com',
            'contact_telephone': '1234444444',
            'display_name': 'milk',
            'hearing_role': 'Litigant in person',
            'user_role': 'Individual',
            'case_group_type': 'claimant',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            }
            ]
            }
            }";
            await _sut.Run(message, new LoggerFake());

            _videoApiService.BookNewConferenceCount.Should().Be(0);
        }

        [Test]
        public async Task Should_handle_hearing_datetime_changed_integration_event()
        {
            const string message = @"{
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
            'id': '71d5c03b-6f19-430b-805d-ce5778a96240',
            'timestamp': '2022-06-21T09:41:55.2323374Z',
            'integration_event': {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingDateTimeChangedIntegrationEvent, BookingsApi.Infrastructure.Services',
            'hearing': {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
            'hearing_id': '2018cdcd-880a-418b-b6eb-0237e2f828a8',
            'group_id': null,
            'scheduled_date_time': '2022-06-25T11:00:26.507Z',
            'scheduled_duration': 50,
            'case_type': 'Civil',
            'case_number': 'SingleDayWithJudge-API',
            'case_name': 'SingleDayWithJudge-API',
            'hearing_venue_name': 'Aberdeen Tribunal Hearing Centre',
            'record_audio': true
            },
            'old_scheduled_date_time': '2022-06-24T14:00:26.507Z',
            'participants': [
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '347035a0-cf21-431c-bdde-23328da2afdb',
            'fullname': 'Mrs Manual_VW Individual_68',
            'username': 'manual_vw.individual_68@hearings.reform.hmcts.net',
            'first_name': 'Manual_VW',
            'last_name': 'Individual_68',
            'contact_email': 'manual_vw.individual_68@hmcts.net',
            'contact_telephone': '+44(0)06713491637',
            'display_name': 'CLIP',
            'hearing_role': 'Litigant in person',
            'user_role': 'Individual',
            'case_group_type': 'claimant',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '4a42aa0b-9df2-4178-945b-73261a552e78',
            'fullname': ' Manual_VW PanelMember_08',
            'username': 'manual_vw.panelmember_08@hearings.reform.hmcts.net',
            'first_name': 'Manual_VW',
            'last_name': 'PanelMember_08',
            'contact_email': 'manual_vw.panelmember_08@hmcts.net',
            'contact_telephone': '+44(0)06713491637',
            'display_name': 'AAD PM',
            'hearing_role': 'Panel Member',
            'user_role': 'Judicial Office Holder',
            'case_group_type': 'panelMember',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '5e61893f-d25f-4e50-8eee-75277f7d3226',
            'fullname': ' Manual_VW Representative_31',
            'username': 'manual_vw.representative_31@hearings.reform.hmcts.net',
            'first_name': 'Manual_VW',
            'last_name': 'Representative_31',
            'contact_email': 'manual_vw.representative_31@hmcts.net',
            'contact_telephone': '+44(0)06713491637',
            'display_name': 'representative_31',
            'hearing_role': 'Representative',
            'user_role': 'Representative',
            'case_group_type': 'defendant',
            'representee': 'DEF',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '74a5b6a3-2430-4080-8ed2-aa17806d42ad',
            'fullname': ' Manchester CFJC Courtroom10',
            'username': 'ManchesterCFJCcourt10@hearings.reform.hmcts.net',
            'first_name': 'Manchester CFJC',
            'last_name': 'Courtroom10',
            'contact_email': 'ManchesterCFJCcourt10@hearings.reform.hmcts.net',
            'contact_telephone': '+44(0)06713491637',
            'display_name': 'Judge James',
            'hearing_role': 'Judge',
            'user_role': 'Judge',
            'case_group_type': 'judge',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': '',
            'contact_phone_for_non_e_jud_judge_user': ''
            }
            ]
            }
            }";
            await _sut.Run(message, new LoggerFake());

            _videoApiService.BookNewConferenceCount.Should().Be(0);
        }
        [Test]
        public async Task Should_handle_multiday_hearing_integration_event()
        {
            const string message = @" {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
            'id': '13be3fe8-6d24-4624-9a9e-2895e7c5dc15',
            'timestamp': '2022-06-20T16:47:20.9793135Z',
            'integration_event': {
            '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.MultiDayHearingIntegrationEvent, BookingsApi.Infrastructure.Services',
            'hearing': {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
            'hearing_id': '74e4b0f2-cbe0-4246-8f25-325d26d8df5e',
            'group_id': '74e4b0f2-cbe0-4246-8f25-325d26d8df5e',
            'scheduled_date_time': '2022-06-22T11:00:00Z',
            'scheduled_duration': 480,
            'case_type': 'Civil',
            'case_number': 'MultiDayWithJudge',
            'case_name': 'MultiDayWithJudge',
            'hearing_venue_name': 'Aberdeen Tribunal Hearing Centre',
            'record_audio': true
            },
            'participants': [
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '82d21eb0-2543-4c42-aee9-17ce7ba29091',
            'fullname': ' Komal Judge',
            'username': 'komal.judge@hearings.reform.hmcts.net',
            'first_name': 'Komal',
            'last_name': 'Judge',
            'contact_email': 'komal.judge@hearings.reform.hmcts.net',
            'contact_telephone': null,
            'display_name': 'Komal Judge',
            'hearing_role': 'Judge',
            'user_role': 'Judge',
            'case_group_type': 'judge',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': 'komal.dabhi@test.net',
            'contact_phone_for_non_e_jud_judge_user': '98738'
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': 'ebd141d8-0046-4472-99ac-57fe2eeed2db',
            'fullname': 'Mr manual observer136',
            'username': 'manual.observer136@hmcts.net',
            'first_name': 'manual',
            'last_name': 'observer136',
            'contact_email': 'manual.observer136@hmcts.net',
            'contact_telephone': '9887544',
            'display_name': 'OBS136',
            'hearing_role': 'Observer',
            'user_role': 'Individual',
            'case_group_type': 'observer',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '7b90e6bd-f4bf-4aed-a0a1-7dd6fd8af8fb',
            'fullname': 'Mrs Manual_VW Individual_09',
            'username': 'manual_vw.individual_09@hearings.reform.hmcts.net',
            'first_name': 'Manual_VW',
            'last_name': 'Individual_09',
            'contact_email': 'manual_vw.individual_09@hmcts.net',
            'contact_telephone': '+44(0)71234567891',
            'display_name': 'LIP',
            'hearing_role': 'Litigant in person',
            'user_role': 'Individual',
            'case_group_type': 'claimant',
            'representee': '',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            },
            {
            '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
            'participant_id': '9a192e70-9068-48f1-9396-d9e5bba6e9d2',
            'fullname': 'Mrs Manual Representative_54',
            'username': 'manual.representative_54@hearings.reform.hmcts.net',
            'first_name': 'Manual',
            'last_name': 'Representative_54',
            'contact_email': 'manual.representative_54@hmcts.net',
            'contact_telephone': '028900800',
            'display_name': 'rgswr',
            'hearing_role': 'Representative',
            'user_role': 'Representative',
            'case_group_type': 'defendant',
            'representee': 'DLIP',
            'linked_participants': [],
            'contact_email_for_non_e_jud_judge_user': null,
            'contact_phone_for_non_e_jud_judge_user': null
            }
            ],
            'total_days': 3
            }
            }";

            await _sut.Run(message, new LoggerFake());

            _videoApiService.BookNewConferenceCount.Should().Be(0);
        }

    }
}