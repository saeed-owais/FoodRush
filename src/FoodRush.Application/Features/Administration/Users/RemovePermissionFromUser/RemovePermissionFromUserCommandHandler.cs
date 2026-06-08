using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;

internal sealed class RemovePermissionFromUserCommandHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<RemovePermissionFromUserCommand, Result>
{
    public async Task<Result> Handle(RemovePermissionFromUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await dbContext.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found."));
        }

        bool permissionExists = await dbContext.Permissions
            .AnyAsync(p => p.Id == request.PermissionId, cancellationToken);

        if (!permissionExists)
        {
            return Result.Failure(Error.NotFound("Permission.NotFound", $"Permission with ID {request.PermissionId} not found."));
        }

        UserPermission? userPermission = await dbContext.UserPermissions
            .AsTracking()
            .FirstOrDefaultAsync(
                up => up.UserId == request.UserId &&
                    up.PermissionId == request.PermissionId,
                cancellationToken);

        if (userPermission == null)
        {
            return Result.Failure(Error.NotFound("UserPermission.NotFound", $"Permission with ID {request.PermissionId} is not assigned to user with ID {request.UserId}."));
        }

        dbContext.UserPermissions.Remove(userPermission);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
