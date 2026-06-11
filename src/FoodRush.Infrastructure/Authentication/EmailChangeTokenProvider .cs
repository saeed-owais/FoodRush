using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class EmailChangeTokenProvider
    (IDataProtectionProvider dataProtectionProvider)
    : IEmailChangeTokenProvider
{

    private readonly IDataProtector dataProtector =
        dataProtectionProvider.CreateProtector("EmailChangeToken");

    public string GenerateToken(EmailChangeTokenPayload payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return dataProtector.Protect(json);
    }

    public EmailChangeTokenPayload? ValidateToken(string token)
    {
        try
        {
            var json = dataProtector.Unprotect(token);

            EmailChangeTokenPayload? payload = JsonSerializer.Deserialize<EmailChangeTokenPayload>(json);

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
