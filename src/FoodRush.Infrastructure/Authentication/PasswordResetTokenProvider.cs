using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class PasswordResetTokenProvider
    (IDataProtectionProvider dataProtectionProvider)
    : IPasswordResetTokenProvider
{
    private readonly DataProtectionTokenProvider<PasswordResetTokenPayload> tokenProvider =
        new(dataProtectionProvider, TokenPurposes.PasswordReset);

    public string GenerateToken(PasswordResetTokenPayload payload)
    => tokenProvider.GenerateToken(payload);

    public PasswordResetTokenPayload? ValidateToken(string token)
    => tokenProvider.ValidateToken(token, payload => payload.ExpiresAt > DateTime.UtcNow);
}
