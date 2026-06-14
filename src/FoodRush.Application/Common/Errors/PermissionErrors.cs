namespace FoodRush.Application.Common.Errors;

internal static class PermissionErrors
{
    public static Error NotFound(Guid permissionId)
    => Error.NotFound(
        "Permission.NotFound",
        $"Permission with ID {permissionId} was not found.");

    public static Error AlreadyExists(string code)
    => Error.Conflict(
        "Permission.AlreadyExists",
        $"Permission with code '{code}' already exists .");
    public static Error AlreadyAssignedToRole(Guid permissionId)
    => Error.Conflict(
        "Permission.Conflict",
        $"Permission with ID {permissionId} is assigned to one or more roles.");
    public static Error AlreadyAssignedToUsers(Guid permissionId)
    => Error.Conflict(
        "Permission.Conflict",
        $"Permission with ID {permissionId} is assigned to one or more users and cannot be deleted.");

    public static Error AlreadyAssignedToUser(Guid permissionId, Guid userId)
    => Error.Conflict(
        "Permission.AlreadyAssigned",
        $"Permission with ID {permissionId} is already assigned to user with ID {userId}.");
}