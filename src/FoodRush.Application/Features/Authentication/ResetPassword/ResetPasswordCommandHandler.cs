using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Authentication.ResetPassword;

internal sealed class ResetPasswordCommandHandler
(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IPasswordResetTokenProvider tokenProvider,
    IRefreshTokenService refreshTokenService,
    ICurrentRequestInfo currentRequestInfo,
    IUserSecurityStampService securityStampService,
    ILogger<ResetPasswordCommandHandler> logger
)
    : IRequestHandler<ResetPasswordCommand, Result>
{

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        PasswordResetTokenPayload? payload = tokenProvider.ValidateToken(request.Token);

        if (payload is null)
        {
            return Result.Failure(AuthErrors.InvalidPasswordResetToken);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(AuthErrors.InvalidPasswordResetToken);
        }

        bool isEmailMatch = string.Equals(user.Email, payload.Email, StringComparison.OrdinalIgnoreCase);

        if (!isEmailMatch)
        {
            return Result.Failure(AuthErrors.InvalidPasswordResetToken);
        }

        if (user.SecurityStamp != payload.SecurityStamp)
        {
            return Result.Failure(AuthErrors.InvalidPasswordResetToken);
        }

        try
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            string newSecurityStamp = Guid.NewGuid().ToString();
            DateTime utcNow = DateTime.UtcNow;
            string? revokedByIp = currentRequestInfo.IpAddress;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await dbContext.Database.BeginTransactionAsync(cancellationToken);

                user.PasswordHash = passwordHasher.Hash(request.NewPassword);
                user.SecurityStamp = newSecurityStamp;

                await refreshTokenService.RevokeAllAsync(
                    user.Id,
                    revokedByIp,
                    utcNow,
                    cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while resetting the password for user with ID {UserId}.",
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
                "An error occurred while updating the security stamp for user with ID {UserId}.",
                user.Id);
        }

        return Result.Success();
    }
}