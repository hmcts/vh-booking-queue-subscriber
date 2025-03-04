using BookingQueueSubscriber.Services.Consts;
using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.MessageHandlers.Extensions;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using LinkedParticipantType = BookingQueueSubscriber.Services.MessageHandlers.Dtos.LinkedParticipantType;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class ParticipantToUpdateParticipantRequestMapperTests
    {
        [TestCase(UserRoleName.Individual)]
        [TestCase(UserRoleName.JudicialOfficeHolder)]
        [TestCase(UserRoleName.StaffMember)]
        public void should_map_participant_dto_to_participant_request(string userRole)
        {
            
            var participantDto = CreateParticipantDto(userRole: userRole);

            var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.DisplayName.Should().Be(participantDto.DisplayName);
            request.Username.Should().Be(participantDto.Username);
            request.UserRole.Should().Be(participantDto.MapUserRoleToContractEnum());
            request.HearingRole.Should().Be(participantDto.HearingRole);
            request.LinkedParticipants.Should().BeEquivalentTo(new List<LinkedParticipantRequest>());
        }

        [Test]
        public void should_map_participant_dto_with_linked_participant_to_participant_request()
        {
            
            var participantDto = CreateParticipantDtoWithLinkedParticipants();

            var request = ParticipantToUpdateParticipantMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.DisplayName.Should().Be(participantDto.DisplayName);
            request.Username.Should().Be(participantDto.Username);
            request.UserRole.Should().Be(UserRole.Individual);
            request.HearingRole.Should().Be(participantDto.HearingRole);
            var linkedParticipant = request.LinkedParticipants[0];
            linkedParticipant.Type.Should().Be((VideoApi.Contract.Enums.LinkedParticipantType)LinkedParticipantType.Interpreter);
            linkedParticipant.LinkedRefId.Should().Be(participantDto.LinkedParticipants[0].LinkedId);
            linkedParticipant.ParticipantRefId.Should().Be(participantDto.LinkedParticipants[0].ParticipantId);
        }
        
        private static ParticipantDto CreateParticipantDto(string userRole = UserRoleName.Individual)
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = userRole)
                .With(x => x.ParticipantId = Guid.NewGuid())
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