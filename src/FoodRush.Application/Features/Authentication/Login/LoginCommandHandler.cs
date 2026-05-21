using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Login;

internal sealed class LoginCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        string normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        var user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(
                    "Auth.AccountDisabled",
                    "Your account is disabled."));
        }

        if (user.LockoutEnd.HasValue &&
            user.LockoutEnd.Value > DateTime.UtcNow)
        {
            TimeSpan lockoutTimeRemaining = user.LockoutEnd.Value - DateTime.UtcNow;
            return Result.Failure<LoginResponse>(
                Error.Unauthorized(
                    "Auth.AccountLocked",
                    $"Your account is locked. Try again in {lockoutTimeRemaining.TotalMinutes} minutes and {lockoutTimeRemaining.Seconds} seconds."));
        }

        bool isPasswordValid = passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Failure<LoginResponse>(
                Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }


        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;


        IEnumerable<string> roles = await dbContext.UserRoles
            .Where(userRole => userRole.UserId == user.Id)
            .Select(userRole => userRole.Role.Code)
            .ToListAsync(cancellationToken);

        TokenResult tokenResult = tokenProvider.GenerateToken(user, roles);

        string refreshTokenValue = tokenProvider.GenerateRefreshToken();

        RefreshToken refreshToken = new()
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new LoginResponse(tokenResult.AccessToken,
            refreshTokenValue,
            tokenResult.ExpiresAtUtc));
    }
}