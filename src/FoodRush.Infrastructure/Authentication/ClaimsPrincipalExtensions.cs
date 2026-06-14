using System.Security.Claims;

namespace FoodRush.Infrastructure.Authentication
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static Guid GetUserId(this ClaimsPrincipal user)
        {
            string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userId, out Guid parsedUserId)
                ? parsedUserId
                : throw new InvalidOperationException("User context is unavailable");
        }
    }
}
