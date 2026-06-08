using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;

internal sealed class AssignPermissionToUserCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<AssignPermissionToUserCommand, Result>
{
    public async Task<Result> Handle(AssignPermissionToUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await _dbContext.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(
                Error.NotFound(
                    "User.NotFound",
                    $"User with ID {request.UserId} not found."));
        }

        bool permissionExists = await _dbContext.Permissions
            .AnyAsync(p => p.Id == request.PermissionId, cancellationToken);

        if (!permissionExists)
        {
            return Result.Failure(
                Error.NotFound(
                    "Permission.NotFound",
                    $"Permission with ID {request.PermissionId} not found."));
        }

        bool alreadyAssigned = await _dbContext.UserPermissions
            .AnyAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId, cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure(
                Error.Conflict(
                    "Permission.AlreadyAssigned",
                    $"Permission with ID {request.PermissionId} is already assigned to user with ID {request.UserId}."));
        }

        UserPermission userPermission = new()
        {
            UserId = request.UserId,
            PermissionId = request.PermissionId
        };

        await _dbContext.UserPermissions.AddAsync(userPermission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
