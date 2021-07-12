using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

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
  '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
  'id': '9250401b-eaec-4b57-81fa-d79026df3e3c',
  'timestamp': '2019-07-02T21:44:11.2088463Z',
  'integration_event': {
    '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.ParticipantsAddedIntegrationEvent, Bookings.Infrastructure.Services',
    'hearing_id': '38bf53e4-32c6-46f1-a7ec-be3016005ec7',
    'participants': [
      {
        '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
        'participant_id': 'a12597ab-b6a1-49a3-a0dd-f15bf05974ab',
        'fullname': 'Mr. Elliott Davis',
        'first_name': 'FirstName1',
        'last_name': 'LastName1',
        'contact_email': 'tst@hmcts.net',
        'contact_telephone': '01234567890',
        'username': 'harley@kshlerin.biz',
        'display_name': 'DisplayName1',
        'hearing_role': 'Litigant in person',
        'user_role': 'Individual',
        'case_group_type': 'Respondent',
        'representee': ''
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
    }
}