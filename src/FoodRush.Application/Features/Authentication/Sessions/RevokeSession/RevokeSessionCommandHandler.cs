using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Sessions.RevokeSession;

internal sealed class RevokeSessionCommandHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    ICurrentRequestInfo currentRequestInfo)
    : IRequestHandler<RevokeSessionCommand, Result>
{
    public async Task<Result> Handle(
        RevokeSessionCommand request,
        CancellationToken cancellationToken)
    {
        RefreshToken? session = await dbContext.RefreshTokens
            .AsTracking()
            .FirstOrDefaultAsync(
                rt =>
                    rt.Id == request.SessionId &&
                    rt.UserId == userContext.UserId,
                cancellationToken);

        if (session is null)
        {
            return Result.Success();
        }

        if (!session.IsActive)
        {
            return Result.Success();
        }

        session.RevokedAt = DateTime.UtcNow;

        session.RevokedByIp =
            currentRequestInfo.IpAddress;

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }
}