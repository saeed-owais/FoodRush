using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.BanUser;

internal sealed class BanUserCommandHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;

        if (request.BanEndDate <= utcNow)
        {
            return Result.Failure(UserErrors.InvalidBanDate);
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        if (user.LockoutEnd > utcNow)
        {
            return Result.Failure(UserErrors.AlreadyBanned(request.UserId, utcNow));
        }

        user.LockoutEnd = request.BanEndDate;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
