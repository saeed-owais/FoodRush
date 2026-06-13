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
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        bool roleExists = await dbContext.Roles
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            return Result.Failure(RoleErrors.NotFound(request.RoleId));
        }

        bool isRoleAssignedToUser = await dbContext.UserRoles
            .AnyAsync(ur =>
                ur.UserId == request.UserId &&
                ur.RoleId == request.RoleId,
            cancellationToken);

        if (isRoleAssignedToUser)
        {
            return Result.Failure(
                RoleErrors.AlreadyAssignedToUser(request.RoleId, request.UserId));
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
