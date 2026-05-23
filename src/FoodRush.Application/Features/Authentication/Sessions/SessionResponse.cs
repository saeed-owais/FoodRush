namespace FoodRush.Application.Features.Authentication.Sessions;

public sealed record SessionResponse(
    Guid Id,
    string? UserAgent,
    string? IpAddress,
    DateTime CreatedAt,
    DateTime? LastUsedAt,
    bool IsCurrentSession);