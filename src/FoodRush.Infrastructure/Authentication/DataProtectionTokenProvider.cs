using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class DataProtectionTokenProvider<TPayload>
    (IDataProtectionProvider dataProtectionProvider,
    string purpose)
{
    private readonly IDataProtector dataProtector = dataProtectionProvider.CreateProtector(purpose);

    public string GenerateToken(TPayload payload)
    {
        string json = JsonSerializer.Serialize(payload);

        return dataProtector.Protect(json);
    }

    public TPayload? ValidateToken(string token, Func<TPayload, bool>? validator = null)
    {
        try
        {
            string json = dataProtector.Unprotect(token);

            TPayload? payload = JsonSerializer.Deserialize<TPayload>(json);

            if (payload is null)
            {
                return default;
            }

            if (validator is not null && !validator(payload))
            {
                return default;
            }

            return payload;
        }
        catch
        {
            return default;
        }
    }
}