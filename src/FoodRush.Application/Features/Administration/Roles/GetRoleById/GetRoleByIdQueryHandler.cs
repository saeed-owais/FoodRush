using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.GetRoleById;

public sealed class GetRoleByIdQueryHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<GetRoleByIdQuery, Result<RoleResponse>>
{
    public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles
            .AsNoTracking()
            .Where(r => r.Id == request.RoleId)
            .Select(r => new RoleResponse(
                r.Id,
                r.Name,
                r.Code))
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null)
        {
            return Result.Failure<RoleResponse>(RoleErrors.NotFound(request.RoleId));
        }

        return new RoleResponse(role.Id, role.Name, role.Code);
    }
}
