using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Authentication.ChangePassword;

internal sealed class ChangePasswordCommandHandler
(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IUserContext userContext,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService,
    IUserSecurityStampService securityStampService,
    ILogger<ChangePasswordCommandHandler> logger
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(userContext.UserId));
        }

        bool isOldPasswordValid = passwordHasher.Verify(request.OldPassword, user.PasswordHash);

        if (!isOldPasswordValid)
        {
            return Result.Failure(AuthErrors.InvalidPassword);
        }

        bool samePassword = passwordHasher.Verify(request.NewPassword, user.PasswordHash);

        if (samePassword)
        {
            return Result.Failure(AuthErrors.SamePassword);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var securityStamp = Guid.NewGuid().ToString();

        try
        {
            user.PasswordHash = passwordHasher.Hash(request.NewPassword);

            user.SecurityStamp = securityStamp;

            DateTime dateTime = DateTime.UtcNow;

            await refreshTokenService.RevokeAllAsync(user.Id, currentRequestInfo.IpAddress, dateTime, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while changing the password for user {UserId}",
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
                "An error occurred while updating the security stamp for user {UserId}",
                userContext.UserId);
        }

        return Result.Success();
    }
}
