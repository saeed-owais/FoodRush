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
    IUserContext userContext,
    ICurrentRequestInfo currentRequestInfo,
    IRefreshTokenService refreshTokenService,
    IUserSecurityStampService securityStampService
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

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        user.SecurityStamp = Guid.NewGuid().ToString();

        DateTime dateTime = DateTime.UtcNow;

        await refreshTokenService.RevokeAllAsync(user.Id, currentRequestInfo.IpAddress, dateTime, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await securityStampService.SetAsync(user.Id, user.SecurityStamp, cancellationToken);

        return Result.Success();
    }
}
