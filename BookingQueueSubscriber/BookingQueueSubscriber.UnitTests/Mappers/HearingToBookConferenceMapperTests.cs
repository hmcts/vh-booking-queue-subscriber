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
            var mapper = new HearingToBookConferenceMapper();
            var hearingDto = CreateHearingDto();

            var request = mapper.MapToBookNewConferenceRequest(hearingDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(hearingDto, options => 
                options
                    .Excluding(o => o.HearingId)
                    .Excluding(o => o.Participants)
                );
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.Participants.Count.Should().Be(hearingDto.Participants.Count);
        }

        private static HearingDto CreateHearingDto()
        {
            var participants = Builder<ParticipantDto>.CreateListOfSize(4)
                .All().With(x => x.UserRole = UserRole.Individual.ToString()).Build();
            var dto = new HearingDto
            {
                HearingId = Guid.NewGuid(),
                CaseNumber = "Test1234",
                CaseType = "Civil Money Claims",
                ScheduledDuration = 60,
                ScheduledDateTime = DateTime.UtcNow,
                Participants = participants
            };
            return dto;
        }
    }
}