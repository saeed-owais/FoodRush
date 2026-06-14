namespace FoodRush.Application.Features.Authentication.GetUserPrfile;

public sealed record GetUserProfileResponse(
    Guid Id,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    string? AvatarUrl,
    bool IsEmailVerified,
    bool IsPhoneVerified,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);