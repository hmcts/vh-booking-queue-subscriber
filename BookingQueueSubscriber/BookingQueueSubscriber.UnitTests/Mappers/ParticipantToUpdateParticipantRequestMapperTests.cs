using System.Collections.Generic;
using System.Linq;
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
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.ContactTelephone.Should().Be(participantDto.ContactTelephone);
            request.Representee.Should().Be(participantDto.Representee);
            request.DisplayName.Should().Be(participantDto.DisplayName);
            request.Username.Should().Be(participantDto.Username);
        }
        
        [Test]
        public void should_map_participant_dto_with_linked_participant_to_participant_request()
        {
            var participantDto = CreateParticipantDtoWithLinkedParticipant();
            var expectedLinked = participantDto.LinkedParticipants.FirstOrDefault();

            var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Fullname.Should().Be(participantDto.Fullname);
            request.FirstName.Should().Be(participantDto.FirstName);
            request.LastName.Should().Be(participantDto.LastName);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.ContactTelephone.Should().Be(participantDto.ContactTelephone);
            request.Representee.Should().Be(participantDto.Representee);
            request.DisplayName.Should().Be(participantDto.DisplayName);
            request.Username.Should().Be(participantDto.Username);
            request.LinkedParticipants.Should().NotBeEmpty();
            
            var linkedParticipant = request.LinkedParticipants.FirstOrDefault();
            linkedParticipant.Fullname.Should().Be(expectedLinked.Fullname);
            linkedParticipant.FirstName.Should().Be(expectedLinked.FirstName);
            linkedParticipant.LastName.Should().Be(expectedLinked.LastName);
            linkedParticipant.ContactEmail.Should().Be(expectedLinked.ContactEmail);
            linkedParticipant.ContactTelephone.Should().Be(expectedLinked.ContactTelephone);
            linkedParticipant.Representee.Should().Be(expectedLinked.Representee);
            linkedParticipant.DisplayName.Should().Be(expectedLinked.DisplayName);
            linkedParticipant.Username.Should().Be(expectedLinked.Username);
            linkedParticipant.LinkedParticipants.Should().BeNull();
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .Build();
        }
        
        private static ParticipantDto CreateParticipantDtoWithLinkedParticipant()
        {
            var interpreter = Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .With(x=> x.CaseGroupType = CaseRoleGroup.Observer)
                .With(x => x.HearingRole = "Interpreter")
                .Build();
            
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .With(x => x.CaseGroupType = CaseRoleGroup.Applicant)
                .With(x => x.HearingRole = "Claimant")
                .With(x => x.LinkedParticipants = new List<ParticipantDto>{ interpreter })
                .Build();
        }
    }
}