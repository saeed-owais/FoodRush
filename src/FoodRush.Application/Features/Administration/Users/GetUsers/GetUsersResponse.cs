namespace FoodRush.Application.Features.Administration.Users.GetUsers;

public sealed record GetUsersResponse(
    Guid Id,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    bool IsActive,
    bool IsEmailVerified,
    int RolesCount,
    DateTime CreatedAt,
    DateTime? LastLoginAt);