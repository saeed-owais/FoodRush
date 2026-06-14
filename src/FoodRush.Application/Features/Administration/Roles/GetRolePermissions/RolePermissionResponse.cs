namespace FoodRush.Application.Features.Administration.Roles.GetRolePermissions;

public sealed record RolePermissionResponse(
    Guid Id,
    string Name,
    string Code);
