using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.UnBanUser;

internal sealed class UnBanUserCommandHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<UnBanUserCommand, Result>
{
    public async Task<Result> Handle(UnBanUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        if (user.LockoutEnd is null ||
            user.LockoutEnd <= DateTime.UtcNow)
        {
            return Result.Failure(UserErrors.NotBanned(request.UserId));
        }

        user.LockoutEnd = null;

        user.FailedLoginAttempts = 0;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
