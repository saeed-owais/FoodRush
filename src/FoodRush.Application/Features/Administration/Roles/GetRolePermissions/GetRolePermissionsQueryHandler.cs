using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.GetRolePermissions;

internal sealed class GetRolePermissionsQueryHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<GetRolePermissionsQuery, Result<List<RolePermissionResponse>>>
{
    public async Task<Result<List<RolePermissionResponse>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        bool roleExists = await _dbContext.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            return Result.Failure<List<RolePermissionResponse>>(RoleErrors.NotFound(request.RoleId));
        }

        var rolePermissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == request.RoleId)
            .Select(rp => new RolePermissionResponse(
                Id: rp.PermissionId,
                Name: rp.Permission.Name,
                Code: rp.Permission.Code
            ))
            .ToListAsync(cancellationToken);


        return rolePermissions;
    }
}
