using FoodRush.Domain.Entities.Identity;

namespace FoodRush.Application.Abstractions.Authentication;

public interface IEmailVerificationTokenProvider
{
    string GenerateToken(User user);

    EmailVerificationTokenPayload? ValidateToken(string token);
}
public sealed record EmailVerificationTokenPayload(
    Guid UserId,
    string Email,
    string SecurityStamp,
    DateTime ExpiresAt);