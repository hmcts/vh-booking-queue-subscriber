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
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.Name.Should().Be(participantDto.Fullname);
            request.FirstName.Should().Be(participantDto.FirstName);
            request.LastName.Should().Be(participantDto.LastName);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.ContactTelephone.Should().Be(participantDto.ContactTelephone);
            request.UserRole.ToString().Should().Be(participantDto.UserRole);
            request.HearingRole.Should().Be(participantDto.HearingRole);
            request.CaseTypeGroup.Should().Be(participantDto.CaseGroupType.ToString());
            request.Representee.Should().Be(participantDto.Representee);
            request.LinkedParticipants.Should().BeNull();
        }
        
        [Test]
        public void should_map_participant_dto_with_linked_participant_to_participant_request()
        {
            var participantDto = CreateParticipantDtoWithLinkedParticipant();
            var expectedLinked = participantDto.LinkedParticipants.FirstOrDefault();

            var request = ParticipantToParticipantRequestMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(participantDto, options => 
                options
                    .Excluding(o => o.ParticipantId)
                    .Excluding(o => o.Fullname)
                    .Excluding(o => o.UserRole)
                    .Excluding(o => o.CaseGroupType)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.Name.Should().Be(participantDto.Fullname);
            request.FirstName.Should().Be(participantDto.FirstName);
            request.LastName.Should().Be(participantDto.LastName);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.ContactTelephone.Should().Be(participantDto.ContactTelephone);
            request.UserRole.ToString().Should().Be(participantDto.UserRole);
            request.HearingRole.Should().Be(participantDto.HearingRole);
            request.CaseTypeGroup.Should().Be(participantDto.CaseGroupType.ToString());
            request.Representee.Should().Be(participantDto.Representee);
            
            var linkedParticipant = request.LinkedParticipants.FirstOrDefault();
            linkedParticipant.ParticipantRefId.Should().Be(expectedLinked.ParticipantId);
            linkedParticipant.Name.Should().Be(expectedLinked.Fullname);
            linkedParticipant.FirstName.Should().Be(expectedLinked.FirstName);
            linkedParticipant.LastName.Should().Be(expectedLinked.LastName);
            linkedParticipant.ContactEmail.Should().Be(expectedLinked.ContactEmail);
            linkedParticipant.ContactTelephone.Should().Be(expectedLinked.ContactTelephone);
            linkedParticipant.UserRole.ToString().Should().Be(expectedLinked.UserRole);
            linkedParticipant.HearingRole.Should().Be(expectedLinked.HearingRole);
            linkedParticipant.CaseTypeGroup.Should().Be(expectedLinked.CaseGroupType.ToString());
            linkedParticipant.Representee.Should().Be(expectedLinked.Representee);
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .With(x => x.HearingRole = "Claimant")
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