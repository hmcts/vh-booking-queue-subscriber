using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using VideoApi.Contract.Enums;
using ConferenceRole = VideoApi.Contract.Enums.ConferenceRole;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class HearingToBookConferenceMapperTests
    {
        [Test]
        public void should_map_hearing_dto_to_book_new_conference_request()
        {
            var hearingDto = CreateHearingDto();
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .TheFirst(1).With(x => x.UserRole = UserRole.Judge.ToString())
                .TheNext(2).With(x => x.UserRole = UserRole.Individual.ToString())
                .TheRest().With(x => x.UserRole = UserRole.Representative.ToString())
                .Build();
            var endpoints = CreateEndpoints(participants);

            var request = HearingToBookConferenceMapper
                .MapToBookNewConferenceRequest(hearingDto, participants, endpoints);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(hearingDto, options => 
                options
                    .Excluding(o => o.HearingId).ExcludingMissingMembers()
                );
            request.AudioRecordingRequired.Should().Be(hearingDto.RecordAudio);
            request.AudioPlaybackLanguage.Should().Be(AudioPlaybackLanguage.English);
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.Participants.Count.Should().Be(participants.Count);
            request.Endpoints.Count.Should().Be(endpoints.Count);

            var firstEndpoint = request.Endpoints.First(x => x.DisplayName == "one");
            firstEndpoint.DefenceAdvocate.Should().NotBeEmpty();
            firstEndpoint.ConferenceRole.Should().Be(ConferenceRole.Host);
            
            var thirdEndpoint = request.Endpoints.First(x => x.DisplayName == "three");
            thirdEndpoint.ConferenceRole.Should().Be(ConferenceRole.Guest);
        }

        [Test]
        public void should_map_iswelsh_to_english_and_welsh_playback_language()
        {
            var hearingDto = CreateHearingDto();
            hearingDto.IsVenueWelsh = true;
            
            var request = HearingToBookConferenceMapper
                .MapToBookNewConferenceRequest(hearingDto, new List<ParticipantDto>(), new List<EndpointDto>());
            
            request.AudioPlaybackLanguage.Should().Be(AudioPlaybackLanguage.EnglishAndWelsh);
        }

        private static HearingDto CreateHearingDto()
        {
            var dto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Generic",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true,
                CaseTypeServiceId ="ZZY1",
                VideoSupplier = VideoSupplier.Vodafone,
                IsVenueWelsh = false
            };
            return dto;
        }

        private static List<EndpointDto> CreateEndpoints(IEnumerable<ParticipantDto> participantDtos)
        {
            var rep = participantDtos.First(x => x.UserRole == UserRole.Representative.ToString());

            return new List<EndpointDto>
            {
                new EndpointDto
                {
                    DisplayName = "one", Sip = Guid.NewGuid().ToString(), Pin = "1234", DefenceAdvocateContactEmail = rep.ContactEmail,
                    Role = Services.MessageHandlers.Dtos.ConferenceRole.Host
                },
                new EndpointDto {DisplayName = "two", Sip = Guid.NewGuid().ToString(), Pin = "5678", Role = Services.MessageHandlers.Dtos.ConferenceRole.Host},
                new EndpointDto {DisplayName = "three", Sip = Guid.NewGuid().ToString(), Pin = "9012", Role = Services.MessageHandlers.Dtos.ConferenceRole.Guest}
            };
        }
    }
}