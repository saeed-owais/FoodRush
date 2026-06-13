using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.ResetPassword;

internal sealed class ResetPasswordCommandHandler
(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IPasswordResetTokenProvider tokenProvider,
    IRefreshTokenService refreshTokenService,
    ICurrentRequestInfo currentRequestInfo,
    IUserSecurityStampService securityStampService
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

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        user.SecurityStamp = Guid.NewGuid().ToString();

        DateTime utcNow = DateTime.UtcNow;

        var revokedByIp = currentRequestInfo.IpAddress;

        await refreshTokenService.RevokeAllAsync(user.Id, revokedByIp, utcNow, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        await securityStampService.SetAsync(user.Id, user.SecurityStamp, cancellationToken);

        return Result.Success();
    }
}