using System;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class HearingToBookConferenceMapperTests
    {
        [Test]
        public void should_map_hearing_dto_to_book_new_conference_request()
        {
            var hearingDto = CreateHearingDto();
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build();

            var request = HearingToBookConferenceMapper.MapToBookNewConferenceRequest(hearingDto, participants);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(hearingDto, options => 
                options
                    .Excluding(o => o.HearingId)
                );
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.Participants.Count.Should().Be(participants.Count);
        }

        private static HearingDto CreateHearingDto()
        {
            var dto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Civil Money Claims",
                CaseName = "Automated Case vs Humans",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                HearingVenueName = "MyVenue",
                RecordAudio = true
            };
            return dto;
        }
    }
}