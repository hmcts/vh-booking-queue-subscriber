using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunctionTests;

public class HearingIsReadyForVideoIntegrationMessageTests
{
    private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
    private VideoApiServiceFake _videoApiService;
    private VideoWebServiceFake _videoWebService;
    private BookingQueueSubscriberFunction _sut;
    private ILogger<BookingQueueSubscriberFunction> _logger;
    
    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<BookingQueueSubscriberFunction>>().Object;
        _videoApiService = (VideoApiServiceFake) _serviceProvider.GetService<IVideoApiService>();
        _videoWebService = (VideoWebServiceFake) _serviceProvider.GetService<IVideoWebService>();
        _sut = new BookingQueueSubscriberFunction(new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider), _logger);
    }
    
    [TearDown]
    public void TearDown()
    {
        _videoApiService.ClearRequests();
        _videoWebService.ClearRequests();
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
        await _sut.Run(message);

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
        await _sut.Run(message);

        _videoApiService.BookNewConferenceCount.Should().Be(1);
        _videoWebService.PushNewConferenceAddedMessageCount.Should().Be(1);
    }
}