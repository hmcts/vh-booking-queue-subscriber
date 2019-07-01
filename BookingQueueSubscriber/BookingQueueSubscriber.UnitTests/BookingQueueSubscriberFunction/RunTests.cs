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
  'id': '7d422b62-8c07-4b61-a6e9-6fb3ea1b52e4',
  'timestamp': '2019-06-28T09:46:23.3358216Z',
  'integration_event': {
    '$type': 'Bookings.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, Bookings.Infrastructure.Services',
    'hearing': {
      'hearing_id': 'fa2e1205-83dd-49dd-9847-42b60c468928',
      'scheduled_date_time': '2019-06-28T23:00:00Z',
      'scheduled_duration': 1,
      'case_type': 'Civil Money Claims',
      'case_number': 'Number1',
      'case_name': 'Name1'
    },
    'participants': [
      {
        'participant_id': 'db016c48-041b-435c-9a1a-a5396a0b77f6',
        'fullname': 'Title1 FirstName1 LastName1',
        'username': 'zackary_beahan@cassindibbert.uk',
        'display_name': 'DisplayName1',
        'hearing_role': 'Claimant LIP',
        'user_role': 'Individual',
        'case_group_type': 'claimant',
        'representee': ''
      },
      {
        'participant_id': '24e0a7cf-49d1-4a90-9bce-aa0f706c1f93',
        'fullname': 'Title2 FirstName2 LastName2',
        'username': 'toy_mccullough@hane.co.uk',
        'display_name': 'DisplayName2',
        'hearing_role': 'Solicitor',
        'user_role': 'Representative',
        'case_group_type': 'claimant',
        'representee': 'Representee2'
      },
      {
        'participant_id': '44b5b90e-1c3e-4fd8-937f-d5600d2d3d08',
        'fullname': 'Title3 FirstName3 LastName3',
        'username': 'jordane_haley@balistrerigutkowski.info',
        'display_name': 'DisplayName3',
        'hearing_role': 'Defendant LIP',
        'user_role': 'Individual',
        'case_group_type': 'defendant',
        'representee': ''
      },
      {
        'participant_id': '4f0dbb73-a15f-46e2-ae58-1c7d53882cc7',
        'fullname': 'Title4 FirstName4 LastName4',
        'username': 'tremayne@beckerhoeger.biz',
        'display_name': 'DisplayName4',
        'hearing_role': 'Solicitor',
        'user_role': 'Representative',
        'case_group_type': 'defendant',
        'representee': 'Representee4'
      },
      {
        'participant_id': '68769ca7-6c59-4160-a22c-d2377769dd4c',
        'fullname': 'Title5 FirstName5 LastName5',
        'username': 'kurtis.bartell@flatleyhirthe.us',
        'display_name': 'DisplayName5',
        'hearing_role': 'Judge',
        'user_role': 'Judge',
        'case_group_type': 'judge',
        'representee': ''
      }
    ],
    'event_type': 'hearingIsReadyForVideo'
  }
}
";
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()));
            Assert.DoesNotThrow(() => BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(),
                new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider)));
        }
    }
}