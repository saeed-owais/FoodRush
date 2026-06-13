using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Authentication.Sessions.LogoutAllSessions;

internal sealed class LogoutAllSessionsCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService,
    IUserSecurityStampService securityStampService,
    ILogger<LogoutAllSessionsCommandHandler> logger)
    : IRequestHandler<LogoutAllSessionsCommand, Result>
{
    public async Task<Result> Handle(
        LogoutAllSessionsCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(
                u => u.Id == userContext.UserId,
                cancellationToken);

        if (user is null)
        {
            return Result.Success();
        }

        DateTime utcNow = DateTime.UtcNow;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            user.SecurityStamp =
                Guid.NewGuid().ToString();

            await refreshTokenService.RevokeAllAsync(
                user.Id,
                currentRequestInfo.IpAddress,
                utcNow,
                cancellationToken);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while logging out all sessions for user {UserId}",
                userContext.UserId);

            throw;
        }

        try
        {
            await securityStampService.SetAsync(user.Id, user.SecurityStamp, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "An error occurred while updating security stamp for user {UserId}",
                userContext.UserId);
        }

        return Result.Success();
    }
}