using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.ConfirmEmailChange;

internal sealed class ConfirmEmailChangeCommandHandler
(
    IApplicationDbContext dbContext,
    IEmailChangeTokenProvider tokenProvider,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService
) : IRequestHandler<ConfirmEmailChangeCommand, Result>

{
    public async Task<Result> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
    {
        EmailChangeTokenPayload? payload = tokenProvider.ValidateToken(request.Token);

        if (payload == null)
        {
            return Result.Failure(AuthErrors.InvalidEmailChangeToken);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(payload.UserId));
        }

        if (user.SecurityStamp != payload.SecurityStamp)
        {
            return Result.Failure(AuthErrors.InvalidEmailChangeToken);
        }

        string normalizedEmail = payload.NewEmail.Trim().ToUpperInvariant();

        bool emailExists = await dbContext.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != user.Id, cancellationToken);

        if (emailExists)
        {
            return Result.Failure(UserErrors.EmailAlreadyExists);
        }

        user.Email = payload.NewEmail;

        user.NormalizedEmail = normalizedEmail;

        user.IsEmailVerified = true;

        user.SecurityStamp = Guid.NewGuid().ToString();

        await refreshTokenService.RevokeAllAsync(
            user.Id,
            currentRequestInfo.IpAddress,
            DateTime.UtcNow,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
