using System;
using System.Collections.Generic;
using System.Linq;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using LinkedParticipantType = BookingQueueSubscriber.Services.MessageHandlers.Dtos.LinkedParticipantType;

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
            request.LinkedParticipants.Should().BeEquivalentTo(new List<LinkedParticipantRequest>());
        }
        
        [Test]
        public void should_map_participant_dto_with_linked_participant_to_participant_request()
        {
            
            var participantDto = CreateParticipantDtoWithLinkedParticipants();

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
            var linkedParticipant = request.LinkedParticipants.FirstOrDefault();
            linkedParticipant.Type.Should().Be(LinkedParticipantType.Interpreter);
            linkedParticipant.LinkedRefId.Should().Be(participantDto.LinkedParticipants[0].LinkedId);
            linkedParticipant.ParticipantRefId.Should().Be(participantDto.LinkedParticipants[0].ParticipantId);
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .Build();
        }

        private static ParticipantDto CreateParticipantDtoWithLinkedParticipants()
        {
            var participant = CreateParticipantDto();
            var linkedParticipant = new LinkedParticipantDto
            {
                LinkedId = Guid.NewGuid(),
                ParticipantId = participant.ParticipantId,
                Type = LinkedParticipantType.Interpreter
            };
            participant.LinkedParticipants = new List<LinkedParticipantDto>{linkedParticipant};
            return participant;
        }
    }
}