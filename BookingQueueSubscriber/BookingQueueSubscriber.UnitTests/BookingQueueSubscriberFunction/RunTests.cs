using System;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunction
{
    public class RunTests : MessageHandlerTestBase
    {
        private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
        private VideoApiServiceFake _videoApiService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
          _videoApiService = (VideoApiServiceFake) _serviceProvider.GetService<IVideoApiService>();
        }

        [TearDown]
        public void TearDown()
        {
          _videoApiService.ClearRequests();
        }

        [Test]
        public async Task should_handle_hearing_ready_for_video_integration_event()
        {
            var message = @"
            {
              '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
              'id': '46c99f98-e3b4-440b-a3f3-bd8733ef1b7d',
              'timestamp': '2019-07-01T14:15:52.0898041Z',
              'integration_event': {
                '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, Bookings.Infrastructure.Services',
                'hearing': {
                  '$type': 'Bookings.Infrastructure.Services.Dtos.HearingDto, Bookings.Infrastructure.Services',
                  'hearing_id': '719817d3-5a20-40c5-bfe6-afce48ed48f3',
                  'scheduled_date_time': '2019-07-01T23:00:00Z',
                  'scheduled_duration': 1,
                  'case_type': 'Civil Money Claims',
                  'case_number': 'Number1',
                  'case_name': 'Name1'
                },
                'participants': [
                  {
                    '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
                    'participant_id': '59bde27c-8684-45ca-8376-9183b85dc584',
                    'fullname': 'Title1 FirstName1 LastName1',
                    'username': 'angie_bartell@jastmraz.biz',
                    'display_name': 'DisplayName1',
                    'hearing_role': 'Claimant LIP',
                    'user_role': 'Individual',
                    'case_group_type': 'claimant',
                    'representee': ''
                  },
                  {
                    '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
                    'participant_id': 'f260f24d-177f-4192-828f-09756c63be75',
                    'fullname': 'Title2 FirstName2 LastName2',
                    'username': 'remington.dibbert@stehr.info',
                    'display_name': 'DisplayName2',
                    'hearing_role': 'Solicitor',
                    'user_role': 'Representative',
                    'case_group_type': 'claimant',
                    'representee': 'Representee2'
                  },
                  {
                    '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
                    'participant_id': 'f69a902f-d5d2-4d4f-968f-3028201396a7',
                    'fullname': 'Title3 FirstName3 LastName3',
                    'username': 'samir.mclaughlin@ankunding.ca',
                    'display_name': 'DisplayName3',
                    'hearing_role': 'Defendant LIP',
                    'user_role': 'Individual',
                    'case_group_type': 'defendant',
                    'representee': ''
                  },
                  {
                    '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
                    'participant_id': '9bbcb92b-3d77-4182-b379-792cdf9e9712',
                    'fullname': 'Title4 FirstName4 LastName4',
                    'username': 'sister@marvin.info',
                    'display_name': 'DisplayName4',
                    'hearing_role': 'Solicitor',
                    'user_role': 'Representative',
                    'case_group_type': 'defendant',
                    'representee': 'Representee4'
                  },
                  {
                    '$type': 'Bookings.Infrastructure.Services.Dtos.ParticipantDto, Bookings.Infrastructure.Services',
                    'participant_id': '98bbaa74-af7c-48c9-a9ad-ac8c61423dfe',
                    'fullname': 'Title5 FirstName5 LastName5',
                    'username': 'lloyd_spinka@miller.com',
                    'display_name': 'DisplayName5',
                    'hearing_role': 'Judge',
                    'user_role': 'Judge',
                    'case_group_type': 'judge',
                    'representee': ''
                  }
                ],
                'event_type': 'hearingIsReadyForVideo'
              }
            }";
            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));

            _videoApiService.BookNewConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task should_handle_hearing_cancelled_integration_event()
        {
            var message = @"{
              '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.EventMessage, Bookings.Infrastructure.Services',
              'id': '2e96da15-e99f-4f5c-aaf1-42f853513d63',
              'timestamp': '2019-07-01T14:03:58.0834843Z',
              'integration_event': {
                '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.HearingCancelledIntegrationEvent, Bookings.Infrastructure.Services',
                'hearing_id': 'e33bae78-0cae-4858-a5f0-5134113f1f67',
                'event_type': 'hearingCancelled'
              }
            }";

            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            
            _videoApiService.DeleteConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task should_handle_hearing_details_updated_integration_event()
        {
            var message = @"{
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
      'case_type': 'Civil Money Claims',
      'case_number': 'CaseNumber',
      'case_name': 'CaseName'
    }
  }
}";
            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            _videoApiService.UpdateConferenceCount.Should().Be(1);
        }


        [Test]
        public async Task should_handle_participants_added_integration_event()
        {
            var message = @"{
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
        'username': 'harley@kshlerin.biz',
        'display_name': 'DisplayName1',
        'hearing_role': 'Defendant LIP',
        'user_role': 'Individual',
        'case_group_type': 'defendant',
        'representee': ''
      }
    ]
  }
}";
            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
              new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            _videoApiService.AddParticipantsToConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task should_handle_participant_removed_integration_event()
        {
            var message = @"{
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
            _videoApiService.ConferenceResponse.HearingRefId = Guid.Parse("015a0b0e-a16d-4076-a2b2-328b1d26212b");
            _videoApiService.ConferenceResponse.Participants[0].RefId = Guid.Parse("ea801426-0ea2-4eab-aaf0-647ae146397a");
            
            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
              new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            _videoApiService.RemoveParticipantFromConferenceCount.Should().Be(1);
        }

        [Test]
        public async Task should_handle_participant_updated_integration_event()
        {
            var message = @"{
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
      'username': 'pinkie_kuhlman@weimannbechtelar.co.uk',
      'display_name': 'Raegan Pollich V',
      'hearing_role': 'Solicitor',
      'user_role': 'Representative',
      'case_group_type': 'defendant',
      'representee': 'Bobby Upton'
    }
  }
}";
            _videoApiService.InitConferenceResponse();
            _videoApiService.ConferenceResponse.Id = Guid.Parse("ab013e39-d159-4836-848e-034d2ebbe37a");
            _videoApiService.ConferenceResponse.HearingRefId = Guid.Parse("769d17f6-85f1-4624-bc07-ffdac8ddb619");
            _videoApiService.ConferenceResponse.Participants[0].RefId = Guid.Parse("af9afb87-5cf8-4813-b3dc-0ea96f77e752");
            
            await BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
              new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            _videoApiService.UpdateParticipantDetailsCount.Should().Be(1);
        }
    }
}