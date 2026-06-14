namespace FoodRush.Application.Features.Authentication;

public sealed record TokenResult(string AccessToken, string JwtId, DateTime ExpiresAtUtc);
