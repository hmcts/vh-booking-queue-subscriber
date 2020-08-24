using System;
using System.Collections.Generic;
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
            var endpoints = CreateEndpoints();

            var request = HearingToBookConferenceMapper
                .MapToBookNewConferenceRequest(hearingDto, participants, endpoints);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(hearingDto, options => 
                options
                    .Excluding(o => o.HearingId).ExcludingMissingMembers()
                );
            request.AudioRecordingRequired.Should().Be(hearingDto.RecordAudio);
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.Participants.Count.Should().Be(participants.Count);
            request.Endpoints.Count.Should().Be(endpoints.Count);
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

        private List<EndpointDto> CreateEndpoints()
        {
            return new List<EndpointDto>
            {
                new EndpointDto{DisplayName = "one", Sip = Guid.NewGuid().ToString(), Pin = "1234"},
                new EndpointDto{DisplayName = "two", Sip = Guid.NewGuid().ToString(), Pin = "5678"},
                new EndpointDto{DisplayName = "three", Sip = Guid.NewGuid().ToString(), Pin = "9012"}
            };
        }
    }
}