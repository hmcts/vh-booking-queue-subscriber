using BookingQueueSubscriber.Services.Consts;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Consts;

namespace BookingQueueSubscriber.UnitTests.V2Tests;

public static class HearingEventBuilders
{
    /// <summary>
    /// Create a hearing dto with a judge, individual, representative and judicial office holder
    /// </summary>
    /// <returns></returns>
    public static HearingDto CreateHearing()
    {
        return new HearingDto()
        {
            HearingId = Guid.NewGuid(),
            CaseNumber = "Case Number 1",
            CaseName = "Case Name 1",
            CaseType = "Standard",
            RecordAudio = false,
            ScheduledDateTime = DateTime.UtcNow.AddMinutes(5),
            ScheduledDuration = 30,
            HearingVenueName = "Made Up Venue",
            HearingType = "Standard",
            GroupId = null
        };
    }

    public static List<ParticipantDto> CreateListOfParticipantOfEachType()
    {
        return new List<ParticipantDto>()
        {
            CreateJudge(),
            CreateIndividual(),
            CreateRepresentative(),
            CreateJudicialOfficeHolder()
        };
    }

    private static ParticipantDto CreateJudicialOfficeHolder()
    {
        return new ParticipantDto
        {
            ParticipantId = Guid.NewGuid(),
            Username = "joh@judiciary.hmcts.net",
            ContactEmail = "joh@judiciary.hmcts.net",
            FirstName = "Judiciary",
            HearingRole = HearingRoleName.PanelMember,
            LastName = "Member",
            ContactTelephone = "0123456789",
            UserRole = UserRoleName.JudicialOfficeHolder,
            DisplayName = "Johnny",
        };
    }

    private static ParticipantDto CreateRepresentative()
    {
        return new ParticipantDto
        {
            ParticipantId = Guid.NewGuid(),
            Username = "rep@test.com",
            ContactEmail = "rep@test.com",
            FirstName = "Representative",
            HearingRole = HearingRoleName.Representative,
            LastName = "Doe",
            ContactTelephone = "0123456789",
            UserRole = UserRoleName.Representative,
            DisplayName = "Johnny",
        };
    }

    private static ParticipantDto CreateIndividual()
    {
        return new ParticipantDto
        {
            ParticipantId = Guid.NewGuid(),
            Username = "lip@test.com",
            ContactEmail = "lip@test.com",
            FirstName = "Individual",
            HearingRole = HearingRoleName.LitigantInPerson,
            LastName = "Doe",
            ContactTelephone = "0123456789",
            UserRole = UserRoleName.Individual,
            DisplayName = "Johnny",
        };
    }

    private static ParticipantDto CreateJudge()
    {
        return new ParticipantDto
        {
            ParticipantId = Guid.NewGuid(),
            Username = "judge@judiciary.hmcts.net",
            ContactEmail = "judge@judiciary.hmcts.net",
            FirstName = "Judge",
            HearingRole = HearingRoleName.Judge,
            LastName = "Fudge",
            ContactTelephone = "0123456789",
            UserRole = UserRoleName.Judge,
            DisplayName = "Johnny",
        };
    }
}