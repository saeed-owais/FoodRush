using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class RoleSeeder
{
    public static async Task SeedRolesAsync(
    ApplicationDbContext dbContext,
    CancellationToken cancellationToken)
    {
        if (await dbContext.Roles.AnyAsync(cancellationToken))
        {
            return;
        }

        List<Role> roles =
        [
            new()
        {
            Name = "Super Admin",
            Code = "SUPER_ADMIN"
        },

        new()
        {
            Name = "Admin",
            Code = "ADMIN"
        },

        new()
        {
            Name = "Customer",
            Code = "CUSTOMER"
        },

        new()
        {
            Name = "Driver",
            Code = "DRIVER"
        },

        new()
        {
            Name = "Restaurant Owner",
            Code = "RESTAURANT_OWNER"
        }
        ];

        await dbContext.Roles.AddRangeAsync(
            roles,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
