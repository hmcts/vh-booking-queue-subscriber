using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.NotificationApi;
using NotificationApi.Contract;

namespace BookingQueueSubscriber.UnitTests.Mappers.NotificationMappers
{
    public class MapToHearingAmendmentNotificationTests
    {

        [Test]
        public void should_map_to_ejud_joh_hearing_amendment_notification()
        {
            //Arrange
            const NotificationType expectedNotificationType = NotificationType.HearingAmendmentEJudJoh;
            var hearing = GetHearingDto();
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            ParticipantDto participant = GetParticipantDto("Judicial Office Holder");

            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            //Act
            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, true);

            //Assert
            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_ejud_judge_hearing_amendment_notification()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentEJudJudge;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            ParticipantDto participant = GetParticipantDto("Judge");
            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judge", $"{participant.DisplayName}");
            participant.ContactEmail = "user@judiciarytest.com";

            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_judge_hearing_amendment_notification()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentJudge;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Judge");
            participant.Username = "judge@heairng.net";
            participant.ContactEmailForNonEJudJudgeUser = "judge@hmcts.net";
            participant.ContactPhoneForNonEJudJudgeUser = "123456789";

            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judge", $"{participant.DisplayName}");
            expectedParameters.Add("courtroom account username", participant.Username);
            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmailForNonEJudJudgeUser);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactPhoneForNonEJudJudgeUser);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_lip_hearing_amendment_notification()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentLip;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Individual");

            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("name", $"{participant.FirstName} {participant.LastName}");
            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_representative_hearing_amendment_notification()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentRepresentative;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Representative");
            participant.Representee = "random person";

            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("client name", $"{participant.Representee}");
            expectedParameters.Add("solicitor name", $"{participant.FirstName} {participant.LastName}");
            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_joh_hearing_amendment_notification()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentJoh;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Judicial Office Holder");
            participant.Username = "judge@heairng.net";
            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, false);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_joh_hearing_amendment_notification_has_ejud_username()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentEJudJoh;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Judicial Office Holder");
            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        [Test]
        public void should_map_to_joh_hearing_amendment_notification_has_no_ejud_username()
        {
            var hearing = GetHearingDto();
            var expectedNotificationType = NotificationType.HearingAmendmentJoh;
            var oldDate = new DateTime(2020, 2, 10, 11, 30, 0, DateTimeKind.Utc);
            var newDate = new DateTime(2020, 10, 12, 13, 10, 0, DateTimeKind.Utc);
            var participant = GetParticipantDto("Judicial Office Holder");
            participant.Username = "jj@jj.com";
            var expectedParameters = GetExpectedParameters(hearing);
            expectedParameters.Add("judicial office holder", $"{participant.FirstName} {participant.LastName}");

            var result = AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant, oldDate, newDate, true);

            result.Should().NotBeNull();
            result.HearingId.Should().Be(hearing.HearingId);
            result.ParticipantId.Should().Be(participant.ParticipantId);
            result.ContactEmail.Should().Be(participant.ContactEmail);
            result.NotificationType.Should().Be(expectedNotificationType);
            result.MessageType.Should().Be(MessageType.Email);
            result.PhoneNumber.Should().Be(participant.ContactTelephone);
            result.Parameters.Should().BeEquivalentTo(expectedParameters);
        }

        private static HearingDto GetHearingDto()
        {
            return new HearingDto
            {
                HearingId = Guid.NewGuid(),
                ScheduledDateTime = new DateTime(2020, 2, 10, 12, 15, 0, DateTimeKind.Utc),
                CaseName = "1234",
                CaseNumber = "MBFY/17364",
                CaseType = "test"
            };
        }
        private static Dictionary<string, string> GetExpectedParameters(HearingDto hearing)
        {
            return new Dictionary<string, string>
            {
                {"case name", hearing.CaseName},
                {"case number", hearing.CaseNumber},
                {"Old time", "11:30 AM"},
                {"New time", "2:10 PM"},
                {"Old Day Month Year", "10 February 2020"},
                {"New Day Month Year", "12 October 2020"}
            };
        }

        private static ParticipantDto GetParticipantDto(string userRole)
        {
            return new ParticipantDto
            {
                ParticipantId = Guid.NewGuid(),
                Username = "contact@judiciary.hmcts.net",
                ContactEmail = "contact@judiciary.hmcts.net",
                FirstName = "John",
                HearingRole = "hearingrolename",
                LastName = "Doe",
                ContactTelephone = "0123456789",
                UserRole = userRole,
                DisplayName = "Johnny",
            };
        }


    }
}