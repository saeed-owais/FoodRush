namespace FoodRush.Application.Features.Authentication.Refresh;

public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc);

