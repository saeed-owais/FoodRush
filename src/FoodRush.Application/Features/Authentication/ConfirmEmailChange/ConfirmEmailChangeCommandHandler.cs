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
    IEmailChangeTokenProvider tokenProvider
) : IRequestHandler<ConfirmEmailChangeCommand, Result>

{
    private static readonly Error InvalidToken =
    Error.Validation(
        "EmailChange.InvalidToken",
        "The email change token is invalid or expired.");

    public async Task<Result> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
    {
        EmailChangeTokenPayload? payload = tokenProvider.ValidateToken(request.Token);

        if (payload == null)
        {
            return Result.Failure(InvalidToken);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {payload.UserId} not found.")
            );
        }

        if (user.SecurityStamp != payload.SecurityStamp)
        {
            return Result.Failure(InvalidToken);
        }

        string normalizedEmail = payload.NewEmail.Trim().ToUpperInvariant();

        bool emailExists = await dbContext.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != user.Id, cancellationToken);

        if (emailExists)
        {
            return Result.Failure(
                Error.Conflict("Email.AlreadyInUse", "The provided email is already in use by another account."));
        }

        user.Email = payload.NewEmail;

        user.NormalizedEmail = normalizedEmail;

        user.IsEmailVerified = true;

        user.SecurityStamp = Guid.NewGuid().ToString();

        await dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
            .ExecuteUpdateAsync(
            setters => setters.SetProperty(u => u.RevokedAt, DateTime.UtcNow),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
