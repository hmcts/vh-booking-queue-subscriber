using Microsoft.Extensions.Logging;

namespace BookingQueueSubscriber.Common.Logging;

public static partial class UserServiceLogger
{
    [LoggerMessage(EventId = 2201, Level = LogLevel.Information, Message = "User with contact email {ContactEmail} does not exist. Creating an account.")]
    public static partial void UserDoesNotExist(this ILogger logger, string contactEmail);

    [LoggerMessage( EventId= 2202, Level = LogLevel.Error, Message = "User with contact email {ContactEmail} does not exist. Creating an account. Second try.")]
    public static partial void CreateUserSecondTryError(this ILogger logger, Exception ex, string contactEmail);

    [LoggerMessage( EventId= 2203, Level = LogLevel.Information, Message = "Assigning the user to the group based on the user role {UserRole}")]
    public static partial void AssignUserToGroup(this ILogger logger, string userRole);

    [LoggerMessage( EventId= 2204, Level = LogLevel.Information, Message = "Attempting to create an AD user with contact email {ContactEmail}.")]
    public static partial void CreatingAdUser(this ILogger logger, string contactEmail);

    [LoggerMessage(EventId= 2205, Level = LogLevel.Debug, Message = "Successfully created an AD user with contact email {ContactEmail}.")]
    public static partial void CreatedAdUser(this ILogger logger, string contactEmail);

    [LoggerMessage(EventId= 2206, Level = LogLevel.Information, Message = "Attempt to get username by contact email {ContactEmail}.")]
    public static partial void GettingUserByEmail(this ILogger logger, string contactEmail);

    [LoggerMessage(EventId= 2207, Level = LogLevel.Information, Message = "User with contact email {ContactEmail} found.")]
    public static partial void UserFoundByEmail(this ILogger logger, string contactEmail);

    [LoggerMessage(EventId= 2208, Level = LogLevel.Warning, Message = "User with contact email {ContactEmail} not found.")]
    public static partial void UserNotFoundWarning(this ILogger logger, Exception ex, string contactEmail);

    [LoggerMessage(EventId= 2209, Level = LogLevel.Debug, Message = "{Username} to group {Group}.")]
    public static partial void AddedUserToGroup(this ILogger logger, string username, string group);
}