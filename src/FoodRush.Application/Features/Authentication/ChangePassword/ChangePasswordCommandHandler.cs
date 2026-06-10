using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.ChangePassword;

internal sealed class ChangePasswordCommandHandler
(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IUserContext userContext
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.Unauthorized("Auth.Unauthorized", $"User with ID {userContext.UserId} is not authorized.")
            );
        }

        bool isOldPasswordValid = passwordHasher.Verify(request.OldPassword, user.PasswordHash);

        if (!isOldPasswordValid)
        {
            return Result.Failure(
                Error.Validation("Auth.InvalidPassword", "The provided old password is incorrect.")
            );
        }

        bool samePassword = passwordHasher.Verify(request.NewPassword, user.PasswordHash);

        if (samePassword)
        {
            return Result.Failure(
                Error.Validation("User.SamePassword", "The new password must be different from the old password.")
            );
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        user.SecurityStamp = Guid.NewGuid().ToString();

        DateTime dateTime = DateTime.UtcNow;

        await dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
            .ExecuteUpdateAsync(
               s => s.SetProperty(rt => rt.RevokedAt, dateTime),
                cancellationToken
            );

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
