using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FoodRush.Infrastructure.Authentication
{
    internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId => User?.GetUserId() ??
            throw new InvalidOperationException("User context is unavailable");

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? RefreshToken =>
            httpContextAccessor.HttpContext?
                .Request
                .Cookies["refreshToken"];

        public IReadOnlyCollection<string> Roles =>
            User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value).ToList() ?? [];

        public string? SecurityStamp => User?.FindFirst("security_stamp")?.Value;
    }
}
