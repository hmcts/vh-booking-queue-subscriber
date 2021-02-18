using System;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class HearingToUpdateConferenceMapperTests
    {
        [Test]
        public void should_map_hearing_dto_to_book_new_conference_request()
        {
            var hearingDto = CreateHearingDto();

            var request = HearingToUpdateConferenceMapper.MapToUpdateConferenceRequest(hearingDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(hearingDto, options => 
                options
                    .Excluding(o => o.HearingId).ExcludingMissingMembers()
            );
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.AudioRecordingRequired.Should().Be(hearingDto.RecordAudio);
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
                RecordAudio = true
            };
            return dto;
        }
    }
}