using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.GetRoles;

internal sealed class GetRolesQueryHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<GetRolesQuery, Result<List<RoleResponse>>>
{
    public async Task<Result<List<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        List<RoleResponse> roles =
            await _dbContext.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponse(
                    r.Id,
                    r.Name,
                    r.Code))
                .ToListAsync(cancellationToken);

        return roles;
    }
}
