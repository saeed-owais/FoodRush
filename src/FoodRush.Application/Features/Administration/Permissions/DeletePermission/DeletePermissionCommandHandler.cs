using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Permissions.DeletePermission;

internal sealed class DeletePermissionCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<DeletePermissionCommand, Result>
{
    public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        Permission? permission = await _dbContext.Permissions
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (permission == null)
        {
            return Result.Failure(PermissionErrors.NotFound(request.Id));
        }

        bool isPermissionAssignedToRoles = await _dbContext.RolePermissions
            .AnyAsync(rp => rp.PermissionId == request.Id, cancellationToken);

        if (isPermissionAssignedToRoles)
        {
            return Result.Failure(PermissionErrors.AlreadyAssignedToRole(request.Id));
        }

        bool isPermissionAssignedToUsers = await _dbContext.UserPermissions
            .AnyAsync(up => up.PermissionId == request.Id, cancellationToken);

        if (isPermissionAssignedToUsers)
        {
            return Result.Failure(PermissionErrors.AlreadyAssignedToUsers(request.Id));
        }

        _dbContext.Permissions
            .Remove(permission);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
