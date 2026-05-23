using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Sessions.LogoutAllSessions;

internal sealed class LogoutAllSessionsCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    ICurrentRequestInfo currentRequestInfo)
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

        List<RefreshToken> sessions =
            await dbContext.RefreshTokens
                .AsTracking()
                .Where(rt =>
                    rt.UserId == userContext.UserId &&
                    rt.IsActive)
                .ToListAsync(cancellationToken);

        foreach (RefreshToken session in sessions)
        {
            session.RevokedAt = utcNow;

            session.RevokedByIp =
                currentRequestInfo.IpAddress;
        }

        user.SecurityStamp =
            Guid.NewGuid().ToString();

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}