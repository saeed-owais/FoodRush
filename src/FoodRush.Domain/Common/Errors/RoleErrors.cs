namespace FoodRush.Domain.Common.Errors;

public static class RoleErrors
{
    public static readonly Error CustomerRoleNotFound =
        Error.Failure(
            "Role.RoleNotFound",
            "Customer role was not found.");

    public static Error NotFound(Guid roleId)
    => Error.NotFound(
        "Role.NotFound",
        $"Role with ID {roleId} was not found.");

    public static Error AlreadyExists(string code)
    => Error.Conflict(
        "Role.AlreadyExists",
        $"Role with code '{code}' already exists .");

    public static Error AlreadyAssignedToUsers(Guid permissionId)
    => Error.Conflict(
        "Role.Conflict",
        $"Role with ID {permissionId} is assigned to one or more users and cannot be deleted.");

    public static Error AlreadyAssignedToUser(Guid roleId, Guid userId)
    => Error.Conflict(
        "UserRole.Conflict",
        $"Role with ID {roleId} is already assigned to user with ID {userId}");
}