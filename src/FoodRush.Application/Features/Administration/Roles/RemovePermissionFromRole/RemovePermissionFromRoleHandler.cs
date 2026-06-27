using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.RemovePermissionFromRole;

internal sealed class RemovePermissionFromRoleHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<RemovePermissionFromRoleCommand, Result>
{
    public async Task<Result> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        RolePermission? rolePermission = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(
                rp => rp.RoleId == request.RoleId &&
                rp.PermissionId == request.PermissionId,
                cancellationToken);

        if (rolePermission == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "RolePermission.NotFound",
                    $"Role permission with ID {request.RoleId}-{request.PermissionId} not found."));
        }

        _dbContext.RolePermissions
            .Remove(rolePermission);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

