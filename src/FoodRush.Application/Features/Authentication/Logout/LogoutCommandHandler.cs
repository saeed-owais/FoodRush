using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Logout;

internal sealed class LogoutCommandHandler(
    IApplicationDbContext dbContext,
    IRefreshTokenHasher refreshTokenHasher,
    ICurrentRequestInfo currentRequestInfo)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        string refreshTokenHash = refreshTokenHasher.Hash(request.RefreshToken);

        RefreshToken? refreshToken = await dbContext.RefreshTokens
            .AsTracking()
            .FirstOrDefaultAsync(
            rt => rt.TokenHash == refreshTokenHash,
            cancellationToken);

        if (refreshToken == null)
        {
            return Result.Success();
        }

        if (!refreshToken.IsActive)
        {
            return Result.Success();
        }

        refreshToken.RevokedAt = DateTime.UtcNow;

        string? revokedByIp = currentRequestInfo.IpAddress;
        refreshToken.RevokedByIp = revokedByIp;


        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}