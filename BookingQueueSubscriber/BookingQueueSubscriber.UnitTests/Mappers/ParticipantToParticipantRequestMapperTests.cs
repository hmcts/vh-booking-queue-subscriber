using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class ParticipantToParticipantRequestMapperTests
    {
        [Test]
        public void should_map_participant_dto_to_participant_request()
        {
            var participantDto = CreateParticipantDto();

            var request = ParticipantToParticipantRequestMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(participantDto, options => 
                options
                    .Excluding(o => o.ParticipantId)
                    .Excluding(o => o.Fullname)
                    .Excluding(o => o.UserRole)
                    .Excluding(o => o.CaseGroupType)
                    .Excluding(o => o.HearingRole)
                    .Excluding(o => o.Representee)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.Name.Should().Be(participantDto.Fullname);
            request.UserRole.ToString().Should().Be(participantDto.UserRole);
            request.CaseTypeGroup.Should().Be(participantDto.CaseGroupType.ToString());
            request.Representee.Should().Be(participantDto.Representee);
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .Build();
        }
    }
}