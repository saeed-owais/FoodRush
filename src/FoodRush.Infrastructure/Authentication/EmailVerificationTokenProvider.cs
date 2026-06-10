using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Domain.Entities.Identity;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class EmailVerificationTokenProvider
    (IDataProtectionProvider dataProtectionProvider) : IEmailVerificationTokenProvider
{
    private readonly IDataProtector dataProtector =
        dataProtectionProvider.CreateProtector("FoodRush.EmailVerification");
    public string GenerateToken(User user)
    {
        var payload = new EmailVerificationTokenPayload(
            user.Id,
            user.Email,
            user.SecurityStamp,
            DateTime.UtcNow.AddHours(24));

        var serializedPayload = JsonSerializer.Serialize(payload);

        return dataProtector.Protect(serializedPayload);
    }

    public EmailVerificationTokenPayload? ValidateToken(string token)
    {
        try
        {
            var unprotectedData = dataProtector.Unprotect(token);

            var payload = JsonSerializer.Deserialize<EmailVerificationTokenPayload>(unprotectedData);

            if (payload is null)
            {
                return null;
            }

            if (payload.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            return payload;
        }
        catch
        {
            return null;
        }
    }
}
