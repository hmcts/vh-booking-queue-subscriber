using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Enums;

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
            request.HearingRefId.Should().Be(hearingDto.HearingId);
            request.Participants.Count.Should().Be(participants.Count);
            request.Endpoints.Count.Should().Be(endpoints.Count);
            request.Endpoints.First(x => x.DisplayName == "one").DefenceAdvocate.Should().NotBeEmpty();
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

        private static List<EndpointDto> CreateEndpoints(IEnumerable<ParticipantDto> participantDtos)
        {
            var rep = participantDtos.First(x => x.UserRole == UserRole.Representative.ToString());

            return new List<EndpointDto>
            {
                new EndpointDto
                {
                    DisplayName = "one", Sip = Guid.NewGuid().ToString(), Pin = "1234", DefenceAdvocateContactEmail = rep.ContactEmail
                },
                new EndpointDto {DisplayName = "two", Sip = Guid.NewGuid().ToString(), Pin = "5678"},
                new EndpointDto {DisplayName = "three", Sip = Guid.NewGuid().ToString(), Pin = "9012"}
            };
        }
    }
}