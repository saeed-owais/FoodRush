using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.GetUserRoles;

internal sealed class GetUserRolesQueryHnadler
    (IApplicationDbContext dbContext)
    : IRequestHandler<
        GetUserRolesQuery,
        Result<IReadOnlyCollection<GetUserRoleResponse>>>
{
    public async Task<Result<IReadOnlyCollection<GetUserRoleResponse>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new
            {
                Roles = u.UserRoles.Select(ur => new GetUserRoleResponse(
                    ur.Role.Id,
                    ur.Role.Name,
                    ur.Role.Code
                    )).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result.Failure<IReadOnlyCollection<GetUserRoleResponse>>(
                UserErrors.NotFound(request.UserId));
        }

        IReadOnlyCollection<GetUserRoleResponse> roles = user.Roles.AsReadOnly();
        return Result.Success(roles);
    }
}
