using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class EmailVerificationTokenProvider
    (IDataProtectionProvider dataProtectionProvider) : IEmailVerificationTokenProvider
{
    private readonly DataProtectionTokenProvider<EmailVerificationTokenPayload> tokenProvider =
        new(dataProtectionProvider, TokenPurposes.EmailVerification);

    public string GenerateToken(EmailVerificationTokenPayload payload)
    => tokenProvider.GenerateToken(payload);

    public EmailVerificationTokenPayload? ValidateToken(string token)
    => tokenProvider.ValidateToken(token, payload => payload.ExpiresAt > DateTime.UtcNow);
}


