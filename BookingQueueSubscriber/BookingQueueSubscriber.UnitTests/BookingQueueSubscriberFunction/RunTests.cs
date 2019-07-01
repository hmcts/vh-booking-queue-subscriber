using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using Moq;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunction
{
    public class RunTests : MessageHandlerTestBase
    {

        [Test]
        public void should_handle_hearing_ready_for_video_integration_event()
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
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()));
            Assert.DoesNotThrow(() => BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider)));
        }

        [Test]
        public void should_handle_hearing_cancelled_for_video_integration_event()
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

            BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider));
            //Assert.DoesNotThrow(() => BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
            //    new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider)));
        }
    }
}