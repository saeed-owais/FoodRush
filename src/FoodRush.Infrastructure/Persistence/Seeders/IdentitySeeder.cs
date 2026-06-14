using FoodRush.Application.Abstractions.Authentication;

namespace FoodRush.Infrastructure.Persistence.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
       ApplicationDbContext dbContext,
       IPasswordHasher passwordHasher,
       CancellationToken cancellationToken = default)
    {
        await RoleSeeder.SeedRolesAsync(dbContext, cancellationToken);
        await PermissionSeeder.SeedPermissionsAsync(dbContext, cancellationToken);
        await RolePermissionSeeder.SeedRolePermissionsAsync(dbContext, cancellationToken);
        await SuperAdminSeeder.SeedSuperAdminAsync(dbContext, passwordHasher, cancellationToken);
    }

}