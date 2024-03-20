using BookingQueueSubscriber.Common.Configuration;
using BookingQueueSubscriber.Services.MessageHandlers.Core;
using BookingQueueSubscriber.Services.NotificationApi;
using BookingQueueSubscriber.Services.VideoApi;
using BookingQueueSubscriber.Services.VideoWeb;
using BookingQueueSubscriber.UnitTests.MessageHandlers;
using Microsoft.Extensions.Logging;
using NotificationApi.Contract;

namespace BookingQueueSubscriber.UnitTests.BookingQueueSubscriberFunctionTests;

public class HearingIsReadyForVideoIntegrationMessageTests
{
    private readonly IServiceProvider _serviceProvider = ServiceProviderFactory.ServiceProvider;
    private VideoApiServiceFake _videoApiService;
    private VideoWebServiceFake _videoWebService;
    private NotificationServiceFake _notificationService;
    private BookingQueueSubscriberFunction _sut;
    private ILogger<BookingQueueSubscriberFunction> _logger;
    private FeatureTogglesClientFake _featureToggles;
    
    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<BookingQueueSubscriberFunction>>().Object;
        _videoApiService = (VideoApiServiceFake) _serviceProvider.GetService<IVideoApiService>();
        _videoWebService = (VideoWebServiceFake) _serviceProvider.GetService<IVideoWebService>();
        _notificationService = (NotificationServiceFake) _serviceProvider.GetService<INotificationService>();
        _sut = new BookingQueueSubscriberFunction(new MessageHandlerFactory(ServiceProviderFactory.ServiceProvider), _logger);
        _featureToggles = (FeatureTogglesClientFake)_serviceProvider.GetService<IFeatureToggles>();
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
      var judgeEmail = "automation_judge_judge_1@hmcts.net";
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

    [Test]
    public async Task should_process_a_single_day_hearing_ready_event_with_a_judge_and_participants_feauture_toogle_new_template_on()
    {
      _featureToggles.PostMayTemplateToggle = true;
      string representativeEmail = "user6@email.com";
      string lipEmail = "user2@email.com";
      string judgeEmail = "manual.judge_06@hearings.reform.hmcts.net";
        const string message = @"{
  '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services',
  'id': '1f690c34-353b-4d8b-99a3-e9240088faff',
  'timestamp': '2023-10-25T13:13:10.8432746Z',
  'integration_event': {
    '$type': 'BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services',
    'hearing': {
      '$type': 'BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services',
      'hearing_id': 'a562fc34-6b0a-4ca0-a963-46678ac4573d',
      'group_id': null,
      'scheduled_date_time': '2023-10-30T12:00:00Z',
      'scheduled_duration': 480,
      'case_type': 'Asylum Support',
      'case_number': 'test user 6 10204',
      'case_name': 'Pippo6 10204',
      'hearing_venue_name': 'Bedford County Court and Family Court',
      'record_audio': true,
      'hearing_type': 'Adjourned/Resumed Hearing'
    },
    'participants': [
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '068f8ff6-0800-4cfa-9640-dd5eee0ac744',
        'fullname': ' Manual Judge_06',
        'username': 'manual.judge_06@hearings.reform.hmcts.net',
        'first_name': 'Manual',
        'last_name': 'Judge_06',
        'contact_email': 'manual.judge_06@hearings.reform.hmcts.net',
        'contact_telephone': '011234556789',
        'display_name': 'Manual Judge_06',
        'hearing_role': 'Judge',
        'user_role': 'Judge',
        'case_group_type': 'judge',
        'representee': '',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': true
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '5eb27388-7e02-4ab7-8dca-e70a0e606746',
        'fullname': 'Mr Ma participant',
        'username': 'ma.participant@hearings.reform.hmcts.net',
        'first_name': 'Ma',
        'last_name': 'participant',
        'contact_email': 'user6@email.com',
        'contact_telephone': '+4412346786',
        'display_name': 'user6 rep',
        'hearing_role': 'Representative',
        'user_role': 'Representative',
        'case_group_type': 'appellant',
        'representee': 'user',
        'linked_participants': [],
        'contact_email_for_non_e_jud_judge_user': null,
        'contact_phone_for_non_e_jud_judge_user': null,
        'send_hearing_notification_if_new': true
      },
      {
        '$type': 'BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services',
        'participant_id': '5eb27388-7e02-4ab7-8dca-e70a0e606746',
        'fullname': 'Mr Ma participant2',
        'username': 'ma.participant2@hearings.reform.hmcts.net',
        'first_name': 'Ma',
        'last_name': 'participant',
        'contact_email': 'user2@email.com',
        'contact_telephone': '+4412346786',
        'display_name': 'user6 rep',
        'hearing_role': 'Litigant in person',
        'user_role': 'Individual',
        'case_group_type': 'appellant',
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

        _videoApiService.BookNewConferenceCount.Should().Be(1);
        _videoWebService.PushNewConferenceAddedMessageCount.Should().Be(1);
    }
}