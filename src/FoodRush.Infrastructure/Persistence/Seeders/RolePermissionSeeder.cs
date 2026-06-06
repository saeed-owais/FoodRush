using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class RolePermissionSeeder
{
    public static async Task SeedRolePermissionsAsync(
    ApplicationDbContext dbContext,
    CancellationToken cancellationToken)
    {
        if (await dbContext.RolePermissions.AnyAsync(cancellationToken))
        {
            return;
        }

        Role? superAdminRole =
            await dbContext.Roles
                .FirstAsync(
                    r => r.Code == "SUPER_ADMIN",
                    cancellationToken);

        List<Permission> permissions =
            await dbContext.Permissions
                .ToListAsync(cancellationToken);

        List<RolePermission> rolePermissions =
            permissions
                .Select(permission => new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id
                })
                .ToList();

        await dbContext.RolePermissions.AddRangeAsync(
            rolePermissions,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
