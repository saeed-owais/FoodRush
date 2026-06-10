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
    ITokenProvider tokenProvider,
    IRefreshTokenHasher refreshTokenHasher,
    ICurrentRequestInfo currentRequestInfo)
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

        if (!user.IsEmailVerified)
        {
            return Result.Failure<LoginResponse>(
                Error.Conflict(
                    "Auth.EmailNotVerified",
                    "Please verify your email before signing in."));
        }

        DateTime utcNow = DateTime.UtcNow;

        if (user.LockoutEnd.HasValue &&
            user.LockoutEnd.Value > utcNow)
        {
            TimeSpan lockoutTimeRemaining = user.LockoutEnd.Value - utcNow;
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
                user.LockoutEnd = utcNow.AddMinutes(15);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Failure<LoginResponse>(
                Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid email or password."));
        }


        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = utcNow;


        IEnumerable<string> roles = await dbContext.UserRoles
            .Where(userRole => userRole.UserId == user.Id)
            .Select(userRole => userRole.Role.Code)
            .ToListAsync(cancellationToken);

        var permissions = await
        (
            dbContext.RolePermissions
                .Where(rp => rp.Role.UserRoles
                    .Any(ur => ur.UserId == user.Id))
                .Select(rp => rp.Permission.Code)

            .Union(
                dbContext.UserPermissions
                    .Where(up => up.UserId == user.Id)
                    .Select(up => up.Permission.Code)
            )
        )
        .ToListAsync(cancellationToken);

        TokenResult tokenResult = tokenProvider.GenerateToken(user, roles, permissions);

        string refreshTokenValue = tokenProvider.GenerateRefreshToken();

        string refreshTokenHash = refreshTokenHasher.Hash(refreshTokenValue);

        RefreshToken refreshToken = new()
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            JwtId = tokenResult.JwtId,
            ExpiresAt = utcNow.AddDays(7),
            CreatedByIp = currentRequestInfo.IpAddress,
            UserAgent = currentRequestInfo.UserAgent,
            LastUsedAt = utcNow
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new LoginResponse(tokenResult.AccessToken,
            refreshTokenValue,
            tokenResult.ExpiresAtUtc));
    }
}