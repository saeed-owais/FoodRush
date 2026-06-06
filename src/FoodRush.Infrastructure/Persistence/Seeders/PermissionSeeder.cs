using FoodRush.Application.Common.Authorization;
using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(
    ApplicationDbContext dbContext,
    CancellationToken cancellationToken)
    {
        if (await dbContext.Permissions.AnyAsync(cancellationToken))
        {
            return;
        }

        string[] permissions =
        [
        Permissions.Users.Read,
        Permissions.Users.Create,
        Permissions.Users.Update,
        Permissions.Users.Delete,

        Permissions.Roles.Read,
        Permissions.Roles.Create,
        Permissions.Roles.Update,
        Permissions.Roles.Delete,

        Permissions.PermissionsManagement.Read,
        Permissions.PermissionsManagement.Assign
        ];

        List<Permission> entities =
            permissions
                .Select(permission => new Permission
                {
                    Name = permission,
                    Code = permission
                })
                .ToList();

        await dbContext.Permissions.AddRangeAsync(
            entities,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}

