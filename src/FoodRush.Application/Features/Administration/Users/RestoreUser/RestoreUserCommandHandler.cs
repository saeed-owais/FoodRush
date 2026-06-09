using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.RestoreUser;

internal sealed class RestoreUserCommandHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<RestoreUserCommand, Result>
{
    public async Task<Result> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .IgnoreQueryFilters()
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} was not found"));
        }

        if (!user.IsDeleted)
        {
            return Result.Failure(
                Error.Conflict("User.NotDeleted", $"User with ID {request.UserId} is not deleted"));
        }

        user.IsDeleted = false;
        user.DeletedAt = null;
        user.DeletedBy = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
