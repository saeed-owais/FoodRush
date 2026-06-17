using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Authentication.Refresh;

internal sealed class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    ITokenProvider tokenProvider,
    IRefreshTokenHasher refreshTokenHasher,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService,
    IUserSecurityStampService securityStampService,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        string hashedRefreshToken = refreshTokenHasher.Hash(request.RefreshToken);

        var refreshTokenEntity = await dbContext.RefreshTokens
            .AsTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(
                rt => rt.TokenHash == hashedRefreshToken,
                cancellationToken);

        if (refreshTokenEntity == null)
        {
            return Unauthorized();
        }

        string? ipAddress = currentRequestInfo.IpAddress;

        string? userAgent = currentRequestInfo.UserAgent;

        DateTime utcNow = DateTime.UtcNow;

        if (refreshTokenEntity.IsUsed || refreshTokenEntity.IsRevoked)
        {
            logger.LogWarning(
                "Refresh token reuse detected. UserId={UserId}, TokenId={TokenId}",
                refreshTokenEntity.UserId,
                refreshTokenEntity.Id);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await refreshTokenService.RevokeAllAsync(
                    refreshTokenEntity.UserId,
                    ipAddress,
                    utcNow,
                    cancellationToken);

                refreshTokenEntity.User.SecurityStamp = Guid.NewGuid().ToString(); // Invalidate all existing tokens for the user   

                await dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while revoking tokens for user {UserId}",
                    refreshTokenEntity.UserId);

                return Unauthorized();
            }

            try
            {
                await securityStampService.SetAsync(
                    refreshTokenEntity.UserId,
                    refreshTokenEntity.User.SecurityStamp,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Failed to update security stamp cache for user {UserId}",
                    refreshTokenEntity.UserId);
            }

            return Unauthorized();
        }

        if (refreshTokenEntity.IsExpired)
        {
            return Unauthorized();
        }

        var user = refreshTokenEntity.User;

        if (!user.IsActive)
        {
            return Result.Failure<RefreshTokenResponse>(AuthErrors.UserInactive);
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > utcNow)
        {
            return Result.Failure<RefreshTokenResponse>(AuthErrors.UserLockedOut(user.LockoutEnd.Value - utcNow));
        }

        IEnumerable<string> roles = await dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Code)
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

        string newRefreshToken = tokenProvider.GenerateRefreshToken();

        string newRefreshTokenHash = refreshTokenHasher.Hash(newRefreshToken);

        refreshTokenEntity.UsedAt = utcNow;

        refreshTokenEntity.LastUsedAt = utcNow;

        //refreshTokenEntity.RevokedAt = utcNow;

        refreshTokenEntity.ReplacedByTokenHash = newRefreshTokenHash;

        RefreshToken newRefreshTokenEntity = new()
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = utcNow.AddDays(7),
            JwtId = tokenResult.JwtId,
            CreatedByIp = ipAddress,
            UserAgent = userAgent,
            LastUsedAt = utcNow
        };

        await dbContext.RefreshTokens
            .AddAsync(newRefreshTokenEntity, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new RefreshTokenResponse(
                tokenResult.AccessToken,
                newRefreshToken,
                tokenResult.ExpiresAtUtc
            ));
    }

    private static Result<RefreshTokenResponse> Unauthorized()
    {
        return Result.Failure<RefreshTokenResponse>(AuthErrors.InvalidRefreshToken);
    }
}