using VideoApi.Contract.Enums;

namespace BookingQueueSubscriber.Services.MessageHandlers.Extensions
{
    public static class ParticipantDtoExtensions
    {
        public static UserRole MapUserRoleToContractEnum(this ParticipantDto participantDto)
        {
            return participantDto.UserRole switch
            {
                UserRoleName.JudicialOfficeHolder => UserRole.JudicialOfficeHolder,
                UserRoleName.StaffMember => UserRole.StaffMember,
                _ => Enum.Parse<UserRole>(participantDto.UserRole)
            };
        }
    }
}
