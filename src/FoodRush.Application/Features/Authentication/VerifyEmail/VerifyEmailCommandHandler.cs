using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.VerifyEmail;

internal sealed class VerifyEmailCommandHandler(
    IApplicationDbContext dbContext,
    IEmailVerificationTokenProvider tokenProvider
    ) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        EmailVerificationTokenPayload? payload = tokenProvider.ValidateToken(request.Token);

        if (payload == null)
        {
            return Result.Failure(AuthErrors.InvalidEmailVerifyToken);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(payload.UserId));
        }

        if (user.IsEmailVerified)
        {
            return Result.Success();
        }

        bool isEmailMatch =
            string.Equals(
                user.Email,
                payload.Email,
                StringComparison.OrdinalIgnoreCase);

        if (!isEmailMatch)
        {
            return Result.Failure(AuthErrors.InvalidEmailVerifyToken);
        }

        if (user.SecurityStamp != payload.SecurityStamp)
        {
            return Result.Failure(AuthErrors.InvalidEmailVerifyToken);
        }

        user.IsEmailVerified = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
