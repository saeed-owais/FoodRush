using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Authentication.ConfirmEmailChange;

internal sealed class ConfirmEmailChangeCommandHandler
(
    IApplicationDbContext dbContext,
    IEmailChangeTokenProvider tokenProvider,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService,
    IUserSecurityStampService securityStampService,
    ILogger<ConfirmEmailChangeCommandHandler> logger
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

        user.SecurityStamp = Guid.NewGuid().ToString();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Email = payload.NewEmail;

            user.NormalizedEmail = normalizedEmail;

            user.IsEmailVerified = true;


            await refreshTokenService.RevokeAllAsync(
                user.Id,
                currentRequestInfo.IpAddress,
                DateTime.UtcNow,
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error occurred while confirming email change for user {UserId}",
                user.Id);

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
                "Error occurred while updating security stamp for user {UserId}",
                user.Id);
        }

        return Result.Success();

    }
}
