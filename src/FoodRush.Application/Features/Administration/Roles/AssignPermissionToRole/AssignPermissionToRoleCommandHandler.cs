using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.AssignPermissionToRole;

internal sealed class AssignPermissionToRoleCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<AssignPermissionToRoleCommand, Result>
{
    public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        bool roleExists = await _dbContext.Roles
            .AnyAsync(
                r => r.Id == request.RoleId,
                cancellationToken);

        if (!roleExists)
        {
            return Result.Failure(Error.NotFound("Role.NotFound", $"Role with ID {request.RoleId} was not found."));
        }

        bool permissionExists = await _dbContext.Permissions
            .AnyAsync(
                p => p.Id == request.PermissionId,
                cancellationToken);

        if (!permissionExists)
        {
            return Result.Failure
                (Error.NotFound(
                    "Permission.NotFound",
                    $"Permission with ID {request.PermissionId} was not found."));
        }

        bool alreadyAssigned = await _dbContext.RolePermissions
            .AnyAsync(
                rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId,
                cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure
                (Error.Conflict(
                    "Permission.AlreadyAssigned",
                    $"Permission with ID {request.PermissionId} is already assigned to Role with ID {request.RoleId}."));
        }

        await _dbContext.RolePermissions.AddAsync(
            new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId,
            }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}