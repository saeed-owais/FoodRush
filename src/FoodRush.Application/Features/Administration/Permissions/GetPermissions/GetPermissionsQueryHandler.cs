using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Permissions.GetPermissions;

internal sealed class GetPermissionsQueryHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<GetPermissionsQuery, Result<List<PermissionResponse>>>
{
    public async Task<Result<List<PermissionResponse>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        List<PermissionResponse> permissions =
            await _dbContext.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Code)
                .Select(p => new PermissionResponse(
                    p.Id,
                    p.Name,
                    p.Code))
                .ToListAsync(cancellationToken);

        return permissions;
    }
}
