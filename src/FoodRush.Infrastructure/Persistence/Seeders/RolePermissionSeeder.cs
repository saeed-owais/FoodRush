using FoodRush.Application.Common.Authorization;
using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class RolePermissionSeeder
{
    public static async Task SeedRolePermissionsAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        Role superAdminRole =
            await dbContext.Roles
                .FirstAsync(
                    r => r.Code == Roles.SuperAdmin,
                    cancellationToken);

        List<Permission> permissions =
            await dbContext.Permissions
                .ToListAsync(cancellationToken);

        HashSet<Guid> assignedPermissionIds = await dbContext.RolePermissions
            .Where(rp => rp.RoleId == superAdminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync(cancellationToken);

        List<RolePermission> newRolePermissions = permissions
            .Where(p => !assignedPermissionIds.Contains(p.Id))
            .Select(p => new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = p.Id
            })
            .ToList();

        if (newRolePermissions.Count == 0)
        {
            return;
        }

        await dbContext.RolePermissions.AddRangeAsync(
            newRolePermissions,
            cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);
    }
}