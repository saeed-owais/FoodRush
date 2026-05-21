namespace FoodRush.Application.Features.Authentication;

public sealed record TokenResult(string AccessToken, DateTime ExpiresAtUtc);
