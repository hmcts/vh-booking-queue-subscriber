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
            'id':'35ca2c9f-fde6-4eee-a6e4-3ac9497301d3',
            'timestamp':'2019-05-07T09:35:53.234909Z',
            'integration_event':{
               'hearing':{
                  'hearing_id':'b487fbbc-b214-4f63-9ba9-0f142a6f1801',
                  'scheduled_date_time':'2019-05-07T23:00:00Z',
                  'scheduled_duration':1,
                  'case_type':'Civil Money Claims',
                  'case_number':'Number1',
                  'case_name':'Name1'
               },
               'participants':[
                  {
                     'participant_id':'2b655923-49b9-440b-afdd-3eb0ef5d6d30',
                     'fullname':'Title1 FirstName1 LastName1',
                     'username':'erna_maggio@zboncak.us',
                     'display_name':'DisplayName1',
                     'hearing_role':'Claimant LIP',
                     'user_role':'Individual',
                     'case_group_type':'partyGroup1',
                     'representee':''
                  },
                  {
                     'participant_id':'f0eb5870-18c0-4469-ba06-49e6a0e24729',
                     'fullname':'Title2 FirstName2 LastName2',
                     'username':'al.turcotte@koelpin.ca',
                     'display_name':'DisplayName2',
                     'hearing_role':'Solicitor',
                     'user_role':'Representative',
                     'case_group_type':'partyGroup1',
                     'representee':'Representee2'
                  },
                  {
                     'participant_id':'8fc9b816-29ba-437f-b11f-2ee913e5cefc',
                     'fullname':'Title5 FirstName5 LastName5',
                     'username':'carrie.pouros@veum.info',
                     'display_name':'DisplayName5',
                     'hearing_role':'Judge',
                     'user_role':'Judge',
                     'case_group_type':'partyGroup0',
                     'representee':''
                  }
               ],
               'event_type':'hearingIsReadyForVideo'
            }
         }
";
            VideoApiServiceMock.Setup(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()));
            
            BookingQueueSubscriber.BookingQueueSubscriberFunction.Run(message, new LoggerFake(), new MessageHandlerFactory(MessageHandlersList));
            VideoApiServiceMock.Verify(x => x.BookNewConferenceAsync(It.IsAny<BookNewConferenceRequest>()), Times.Once);
        }
    }
}