using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.MessageHandlers.Extensions;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using LinkedParticipantType = BookingQueueSubscriber.Services.MessageHandlers.Dtos.LinkedParticipantType;

namespace BookingQueueSubscriber.UnitTests.Mappers
{
    public class ParticipantToParticipantRequestMapperTests
    {
        [Test]
        public void should_map_participant_dto_without_linked_participant_to_participant_request()
        {
            var participantDto = CreateParticipantDtoWithoutLinkedParticipants();

            var request = ParticipantToParticipantRequestMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(participantDto, options => 
                options
                    .Excluding(o => o.ParticipantId)
                    .Excluding(o => o.Fullname)
                    .Excluding(o => o.UserRole)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
                    .Excluding(o => o.ContactEmailForNonEJudJudgeUser)
                    .Excluding(o => o.ContactPhoneForNonEJudJudgeUser)
                    .Excluding(o => o.SendHearingNotificationIfNew)
                    .Excluding(o => o.FirstName)
                    .Excluding(o => o.LastName)
                    .Excluding(o => o.ContactTelephone)
                    .Excluding(o => o.Representee)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.UserRole.Should().Be(participantDto.MapUserRoleToContractEnum());
            request.HearingRole.Should().Be(participantDto.HearingRole);
            request.LinkedParticipants.Should().BeEquivalentTo(new List<LinkedParticipantRequest>());
        }
        
        [Test]
        public void should_map_participant_dto_with_linked_participant_to_participant_request()
        {
            var participantDto = CreateParticipantDtoWithLinkedParticipants();

            var request = ParticipantToParticipantRequestMapper.MapToParticipantRequest(participantDto);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(participantDto, options => 
                options
                    .Excluding(o => o.ParticipantId)
                    .Excluding(o => o.Fullname)
                    .Excluding(o => o.UserRole)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
                    .Excluding(o => o.ContactEmailForNonEJudJudgeUser)
                    .Excluding(o => o.ContactPhoneForNonEJudJudgeUser)
                    .Excluding(o => o.SendHearingNotificationIfNew)
                    .Excluding(o => o.FirstName)
                    .Excluding(o => o.LastName)
                    .Excluding(o => o.ContactTelephone)
                    .Excluding(o => o.Representee)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.UserRole.ToString().Should().Be(participantDto.UserRole);
            request.HearingRole.Should().Be(participantDto.HearingRole);
            var linkedParticipant = request.LinkedParticipants[0];
            linkedParticipant.Type.Should().Be((VideoApi.Contract.Enums.LinkedParticipantType)LinkedParticipantType.Interpreter);
            linkedParticipant.LinkedRefId.Should().Be(participantDto.LinkedParticipants[0].LinkedId);
            linkedParticipant.ParticipantRefId.Should().Be(participantDto.LinkedParticipants[0].ParticipantId);
        }

        [Test]
        public void should_map_participant_dto_with_participant_id_and_participant_ref_id_overloads_to_participant_request()
        {
            var participantDto = CreateParticipantDtoWithLinkedParticipants();

            var participantId = Guid.NewGuid();
            var participantRefId = Guid.NewGuid();
            var request = ParticipantToParticipantRequestMapper.MapToParticipantRequest(participantDto, participantId, participantRefId);
            
            request.Should().NotBeNull();
            request.Should().BeEquivalentTo(participantDto, options => 
                options
                    .Excluding(o => o.ParticipantId)
                    .Excluding(o => o.Fullname)
                    .Excluding(o => o.UserRole)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
                    .Excluding(o => o.ContactEmailForNonEJudJudgeUser)
                    .Excluding(o => o.ContactPhoneForNonEJudJudgeUser)
                    .Excluding(o => o.SendHearingNotificationIfNew)
                    .Excluding(o => o.FirstName)
                    .Excluding(o => o.LastName)
                    .Excluding(o => o.ContactTelephone)
                    .Excluding(o => o.Representee)
            );
            request.Id.Should().Be(participantId);
            request.ParticipantRefId.Should().Be(participantRefId);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.UserRole.ToString().Should().Be(participantDto.UserRole);
            request.HearingRole.Should().Be(participantDto.HearingRole);
            var linkedParticipant = request.LinkedParticipants[0];
            linkedParticipant.Type.Should().Be((VideoApi.Contract.Enums.LinkedParticipantType)LinkedParticipantType.Interpreter);
            linkedParticipant.LinkedRefId.Should().Be(participantDto.LinkedParticipants[0].LinkedId);
            linkedParticipant.ParticipantRefId.Should().Be(participantDto.LinkedParticipants[0].ParticipantId);
        }
        
        private static ParticipantDto CreateParticipantDtoWithoutLinkedParticipants()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .With(x => x.HearingRole = "Applicant")
                .Build();
        }
        
        private static ParticipantDto CreateParticipantDtoWithLinkedParticipants()
        {
            var participant = CreateParticipantDtoWithoutLinkedParticipants();
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