using BookingQueueSubscriber.Services.Consts;
using BookingQueueSubscriber.Services.UserApi;
using Microsoft.Extensions.Logging;
using UserApi.Client;
using UserApi.Contract.Requests;

namespace BookingQueueSubscriber.UnitTests.UserServiceTests;

public class AssignUserToGroupTests
{
    private Mock<IUserApiClient> _userApiClientMock;
    private IUserService _userService;
    
    private const string UserId = "UserId";

    [SetUp]
    public void SetUp()
    {
        var loggerMock = new Mock<ILogger<UserService>>();
        _userApiClientMock = new Mock<IUserApiClient>();
        _userService = new UserService(_userApiClientMock.Object, loggerMock.Object);
    }
    
    [Test]
    public async Task Should_assign_representative_user_to_group()
    {
        // Arrange & Act
        await _userService.AssignUserToGroup(UserId, "Representative");

        // Assert
        VerifyUserAddedToGroup(UserGroup.External);
        VerifyUserAddedToGroup(UserGroup.VirtualRoomProfessionalUser);
    }
    
    [Test]
    public async Task Should_assign_joh_user_to_group()
    {
        // Arrange & Act
        await _userService.AssignUserToGroup(UserId, "Judicial Office Holder");

        // Assert
        VerifyUserAddedToGroup(UserGroup.External);
        VerifyUserAddedToGroup(UserGroup.JudicialOfficeHolder);
    }
    
    [Test]
    public async Task Should_assign_staff_member_user_to_group()
    {
        // Arrange & Act
        await _userService.AssignUserToGroup(UserId, "StaffMember");

        // Assert
        VerifyUserAddedToGroup(UserGroup.Internal);
        VerifyUserAddedToGroup(UserGroup.StaffMember);
    }

    [Test]
    public async Task Should_assign_other_user_to_group()
    {
        // Arrange & Act
        await _userService.AssignUserToGroup(UserId, "Interpreter");

        // Assert
        VerifyUserAddedToGroup(UserGroup.External);
    }

    private void VerifyUserAddedToGroup(string groupName)
    {
        _userApiClientMock.Verify(x => x.AddUserToGroupAsync(It.Is<AddUserToGroupRequest>(r => 
            r.UserId == UserId &&
            r.GroupName == groupName)), Times.Once);
    }
}