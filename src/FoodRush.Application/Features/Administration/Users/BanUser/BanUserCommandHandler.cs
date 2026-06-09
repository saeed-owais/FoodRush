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
            return Result.Failure(
                Error.Validation(
                    "Ban.InvalidDate",
                    "Ban end date must be in the future."));
        }

        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} was not found.")
                );
        }

        if (user.LockoutEnd > utcNow)
        {
            return Result.Failure(
                Error.Conflict(
                    "User.AlreadyBanned",
                    $"User with ID {request.UserId} is already banned until {user.LockoutEnd}.")
                );
        }

        user.LockoutEnd = request.BanEndDate;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
