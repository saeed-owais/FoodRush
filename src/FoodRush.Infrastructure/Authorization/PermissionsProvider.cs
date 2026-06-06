using FoodRush.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Authorization;

internal class PermissionsProvider
{
    private readonly IApplicationDbContext _dbContext;

    public PermissionsProvider(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<HashSet<string>> GetUserPermissions(Guid userId)
    {
        HashSet<string> permissions =
        await _dbContext.UserPermissions
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission.Code)

            .Union(

                _dbContext.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .SelectMany(
                        ur => ur.Role.RolePermissions
                            .Select(rp => rp.Permission.Code))

            )
            .ToHashSetAsync();

        return permissions;
    }
}

