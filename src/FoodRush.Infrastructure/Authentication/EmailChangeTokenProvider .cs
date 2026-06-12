using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class EmailChangeTokenProvider
    (IDataProtectionProvider dataProtectionProvider)
    : IEmailChangeTokenProvider
{

    private readonly DataProtectionTokenProvider<EmailChangeTokenPayload> tokenProvider =
        new(dataProtectionProvider, TokenPurposes.EmailChange);

    public string GenerateToken(EmailChangeTokenPayload payload)
    => tokenProvider.GenerateToken(payload);

    public EmailChangeTokenPayload? ValidateToken(string token)
    => tokenProvider.ValidateToken(token, payload => payload.ExpiresAt > DateTime.UtcNow);

}
