using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Sessions;

internal sealed class GetSessionsQueryHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    IRefreshTokenHasher refreshTokenHasher)
    : IRequestHandler<GetSessionsQuery,
        Result<IReadOnlyCollection<SessionResponse>>>
{
    public async Task<Result<IReadOnlyCollection<SessionResponse>>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        string? currentRefreshToken =
            userContext.RefreshToken;

        string? currentRefreshTokenHash =
            currentRefreshToken is null
                ? null
                : refreshTokenHasher.Hash(currentRefreshToken);

        IReadOnlyCollection<SessionResponse> sessions =
            await dbContext.RefreshTokens
                .AsNoTracking()
                .Where(rt =>
                    rt.UserId == userContext.UserId &&
                    rt.IsActive)
                .OrderByDescending(rt => rt.CreatedAt)
                .Select(rt => new SessionResponse(
                    rt.Id,
                    rt.UserAgent,
                    rt.CreatedByIp,
                    rt.CreatedAt,
                    rt.LastUsedAt,
                    rt.TokenHash == currentRefreshTokenHash
                ))
                .ToListAsync(cancellationToken);

        return Result.Success(sessions);
    }
}