using BookingQueueSubscriber.Services.Mappers;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Contract.Enums;

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
        }
        
        private static ParticipantDto CreateParticipantDto()
        {
            return Builder<ParticipantDto>.CreateNew()
                .With(x => x.UserRole = UserRole.Individual.ToString())
                .With(x => x.HearingRole = "Applicant")
                .Build();
        }
    }
}