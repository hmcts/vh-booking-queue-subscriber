using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.VideoApi.Contracts;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class ParticipantToUpdateParticipantRequestMapperTests
    {
        [Test]
        public void should_map_participant_dto_to_participant_request()
        {
            
            var participantDto = CreateParticipantDto();

            var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Fullname.Should().Be(participantDto.Fullname);
            request.FirstName.Should().Be(participantDto.FirstName);
            request.LastName.Should().Be(participantDto.LastName);
            request.Representee.Should().Be(participantDto.Representee);
            request.DisplayName.Should().Be(participantDto.DisplayName);
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .Build();
        }
    }
}