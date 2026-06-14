namespace FoodRush.Application.Abstractions.Authentication;

public interface IEmailChangeTokenProvider
{
    string GenerateToken(EmailChangeTokenPayload payload);
    EmailChangeTokenPayload? ValidateToken(string token);
}

public sealed record EmailChangeTokenPayload(
    Guid UserId,
    string NewEmail,
    string SecurityStamp,
    DateTime ExpiresAt
);