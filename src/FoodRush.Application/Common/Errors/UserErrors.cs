namespace FoodRush.Application.Common.Errors;

internal static class UserErrors
{
    public static Error NotFound(Guid userId)
        => Error.NotFound(
            "User.NotFound",
            $"User with ID {userId} was not found.");

    public static readonly Error EmailAlreadyVerified =
        Error.Conflict(
            "User.EmailAlreadyVerified",
            "User email is already verified.");

    public static readonly Error LastSuperAdmin =
        Error.Conflict(
            "User.LastSuperAdmin",
            "Cannot delete the last super admin.");

    public static readonly Error InvalidBanDate =
        Error.Validation(
            "User.InvalidBanDate",
            "Ban end date must be in the future.");

    public static readonly Error EmailAlreadyExists =
        Error.Conflict(
            "User.EmailAlreadyExists",
            "A user with the same email already exists.");

    public static readonly Error PhoneAlreadyExists =
        Error.Conflict(
            "User.PhoneAlreadyExists",
            "A user with the same phone number already exists.");

    public static Error AlreadyBanned(Guid userId, DateTime lockoutEnd)
    => Error.Conflict(
            "User.AlreadyBanned",
            $"User with ID {userId} is already banned until {lockoutEnd}.");

    internal static Error NotDeleted(Guid userId)
    => Error.Conflict("User.NotDeleted", $"User with ID {userId} is not deleted");

    internal static Error NotBanned(Guid userId)
    => Error.Conflict("User.NotBanned", $"User with ID {userId} is not banned");
}