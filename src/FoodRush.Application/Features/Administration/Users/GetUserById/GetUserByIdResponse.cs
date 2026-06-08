using FoodRush.Application.Features.Administration.Permissions.GetPermissions;
using FoodRush.Application.Features.Administration.Roles;

namespace FoodRush.Application.Features.Administration.Users.GetUserById;

public sealed record GetUserByIdResponse(
    Guid Id,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    string? AvatarUrl,
    bool IsActive,
    bool IsEmailVerified,
    bool IsPhoneVerified,
    int FailedLoginAttempts,
    DateTime? LockoutEnd,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<RoleResponse> Roles,
    IReadOnlyCollection<PermissionResponse> DirectPermissions,
    IReadOnlyCollection<PermissionResponse> RolePermissions
);