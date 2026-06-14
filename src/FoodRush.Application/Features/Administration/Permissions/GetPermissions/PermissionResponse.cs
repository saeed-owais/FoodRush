namespace FoodRush.Application.Features.Administration.Permissions.GetPermissions;

public sealed record PermissionResponse(
    Guid Id,
    string Name,
    string Code);