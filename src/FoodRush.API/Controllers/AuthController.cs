using FoodRush.API.Extensions;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Application.Features.Authentication.Login;
using FoodRush.Application.Features.Authentication.Refresh;
using FoodRush.Application.Features.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.Problem();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            if (result.IsFailure)
            {
                return result.Problem();
            }

            Response.Cookies.Append(
                "refreshToken",
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

            return Ok(new
            {
                AccessToken = result.Value.AccessToken,
                ExpiresAt = result.Value.ExpiresAtUtc
            });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            string? refreshToken =
                Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result
                    .Failure(
                        Error.Unauthorized(
                            "Auth.MissingRefreshToken",
                            "Refresh token is missing."))
                    .Problem();
            }

            RefreshTokenCommand command =
                new(refreshToken);

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (result.IsFailure)
            {
                return result.Problem();
            }

            Response.Cookies.Append(
                "refreshToken",
                result.Value.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

            return Ok(new
            {
                AccessToken = result.Value.AccessToken,
                ExpiresAt = result.Value.ExpiresAtUtc
            });
        }
    }
}
