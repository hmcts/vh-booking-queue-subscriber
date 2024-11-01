using BookingQueueSubscriber.Services.MessageHandlers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;

namespace BookingQueueSubscriber.UnitTests.MessageHandlers.NotificationRequestHelperTests;

public class BuildExistingUserSingleDayHearingConfirmationRequestTests
{
    [Test]
    public void should_map_hearing_and_participant_dto()
    {
        // Arrange
        var hearingDto = new HearingDto
        {
            HearingId = Guid.NewGuid(),
            CaseName = "CaseName",
            CaseNumber = "CaseNumber",
            ScheduledDateTime = new DateTime(2024, 11, 1, 8, 0, 0, DateTimeKind.Utc)
        };
        var participantDto = new ParticipantDto
        {
            ContactEmail = "contactEmail@email.com",
            ParticipantId = Guid.NewGuid(),
            DisplayName = "DisplayName",
            FirstName = "FirstName",
            LastName = "LastName",
            Username = "username@email.com",
            UserRole = "Judge"
        };

        // Act
        var result = NotificationRequestHelper.BuildExistingUserSingleDayHearingConfirmationRequest(hearingDto, participantDto);

        // Assert
        result.HearingId.Should().Be(hearingDto.HearingId);
        result.ContactEmail.Should().Be(participantDto.ContactEmail);
        result.ParticipantId.Should().Be(participantDto.ParticipantId);
        result.CaseName.Should().Be(hearingDto.CaseName);
        result.CaseNumber.Should().Be(hearingDto.CaseNumber);
        result.DisplayName.Should().Be(participantDto.DisplayName);
        result.Name.Should().Be($"{participantDto.FirstName} {participantDto.LastName}");
        result.Username.Should().Be(participantDto.Username);
        result.RoleName.Should().Be(participantDto.UserRole);
        result.ScheduledDateTime.Should().Be(hearingDto.ScheduledDateTime);
    }
}