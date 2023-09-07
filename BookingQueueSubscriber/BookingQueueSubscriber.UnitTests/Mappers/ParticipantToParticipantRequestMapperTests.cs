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
                    .Excluding(o => o.CaseGroupType)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
                    .Excluding(o => o.ContactEmailForNonEJudJudgeUser)
                    .Excluding(o => o.ContactPhoneForNonEJudJudgeUser)
                    .Excluding(o => o.SendHearingNotificationIfNew)
            );
            request.ParticipantRefId.Should().Be(participantDto.ParticipantId);
            request.Name.Should().Be(participantDto.Fullname);
            request.FirstName.Should().Be(participantDto.FirstName);
            request.LastName.Should().Be(participantDto.LastName);
            request.ContactEmail.Should().Be(participantDto.ContactEmail);
            request.ContactTelephone.Should().Be(participantDto.ContactTelephone);
            request.UserRole.Should().Be(participantDto.MapUserRoleToContractEnum());
            request.HearingRole.Should().Be(participantDto.HearingRole);
            request.CaseTypeGroup.Should().Be(participantDto.CaseGroupType.ToString());
            request.Representee.Should().Be(participantDto.Representee);
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
                    .Excluding(o => o.CaseGroupType)
                    .Excluding(o => o.Representee)
                    .Excluding(o => o.LinkedParticipants)
                    .Excluding(o => o.ContactEmailForNonEJudJudgeUser)
                    .Excluding(o => o.ContactPhoneForNonEJudJudgeUser)
                    .Excluding(o => o.SendHearingNotificationIfNew)
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
            var linkedParticipant = request.LinkedParticipants.First();
            linkedParticipant.Type.Should().Be(LinkedParticipantType.Interpreter);
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