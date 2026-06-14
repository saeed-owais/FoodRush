using FoodRush.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class CurrentRequestInfo
: ICurrentRequestInfo
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentRequestInfo(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();

    public string? UserAgent =>
        _httpContextAccessor.HttpContext?
            .Request
            .Headers["User-Agent"]
            .ToString();
}
