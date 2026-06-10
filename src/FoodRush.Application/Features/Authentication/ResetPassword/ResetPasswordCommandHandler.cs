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
    IPasswordResetTokenProvider tokenProvider
)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    private static readonly Error InvalidToken =
        Error.Validation(
            "PasswordReset.InvalidToken",
            "The password reset token is invalid or expired.");

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        PasswordResetTokenPayload? payload = tokenProvider.ValidateToken(request.Token);

        if (payload is null)
        {
            return Result.Failure(InvalidToken);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(InvalidToken);
        }

        bool isEmailMatch = string.Equals(user.Email, payload.Email, StringComparison.OrdinalIgnoreCase);

        if (!isEmailMatch)
        {
            return Result.Failure(InvalidToken);
        }

        if (user.SecurityStamp != payload.SecurityStamp)
        {
            return Result.Failure(InvalidToken);
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);

        user.SecurityStamp = Guid.NewGuid().ToString();

        DateTime utcNow = DateTime.UtcNow;

        await dbContext.RefreshTokens
            .Where(rt =>
                rt.UserId == user.Id &&
                rt.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    rt => rt.RevokedAt,
                    utcNow),
                cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}