using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        UserRole? userRole = await dbContext.UserRoles
            //.AsTracking()
            .FirstOrDefaultAsync(ur =>
                ur.UserId == request.UserId &&
                ur.RoleId == request.RoleId,
            cancellationToken);

        if (userRole == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "UserRole.NotFound",
                    $"The specified role with ID {request.RoleId} is not assigned to the user with ID {request.UserId}.")
            );
        }

        dbContext.UserRoles.Remove(userRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
