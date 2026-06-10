using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class PasswordResetTokenProvider
    (IDataProtectionProvider dataProtectionProvider)
    : IPasswordResetTokenProvider
{
    private readonly IDataProtector dataProtector =
        dataProtectionProvider.CreateProtector("FoodRush.PasswordReset");

    public string GenerateToken(PasswordResetTokenPayload payload)
    {
        var serializedPayload = JsonSerializer.Serialize(payload);
        return dataProtector.Protect(serializedPayload);
    }

    public PasswordResetTokenPayload? ValidateToken(string token)
    {
        try
        {
            var payloadJson = dataProtector.Unprotect(token);

            var payload = JsonSerializer.Deserialize<PasswordResetTokenPayload>(payloadJson);

            if (payload == null)
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
