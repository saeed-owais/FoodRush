using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.AssignRoleToUser;

internal sealed class AssignRoleToUserHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<AssignRoleToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} was not found"));
        }

        bool roleExists = await dbContext.Roles
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            return Result.Failure(
                Error.NotFound("Role.NotFound", $"Role with ID {request.RoleId} was not found"));
        }

        bool isRoleAssignedToUser = await dbContext.UserRoles
            .AnyAsync(ur =>
                ur.UserId == request.UserId &&
                ur.RoleId == request.RoleId,
            cancellationToken);

        if (isRoleAssignedToUser)
        {
            return Result.Failure(
                Error.Conflict("UserRole.Conflict", $"Role with ID {request.RoleId} is already assigned to user with ID {request.UserId}"));
        }

        await dbContext.UserRoles
            .AddAsync(new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
            }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
