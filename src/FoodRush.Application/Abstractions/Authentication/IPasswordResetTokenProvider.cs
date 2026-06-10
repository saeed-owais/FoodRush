namespace FoodRush.Application.Abstractions.Authentication;

public interface IPasswordResetTokenProvider
{
    string GenerateToken(PasswordResetTokenPayload payload);

    PasswordResetTokenPayload? ValidateToken(string token);
}

public sealed record PasswordResetTokenPayload(
    Guid UserId,
    string Email,
    string SecurityStamp,
    DateTime ExpiresAt);