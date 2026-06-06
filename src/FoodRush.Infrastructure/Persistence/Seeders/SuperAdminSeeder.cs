using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class SuperAdminSeeder
{
    public static async Task SeedSuperAdminAsync(
    ApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    CancellationToken cancellationToken)
    {
        const string email =
            "superadmin@foodrush.com";

        bool exists =
            await dbContext.Users
                .AnyAsync(
                    u => u.Email == email,
                    cancellationToken);

        if (exists)
        {
            return;
        }

        User superAdmin = new()
        {
            Email = email,
            NormalizedEmail =
                email.ToUpperInvariant(),

            DisplayName = "FoodRush Super Admin",

            IsActive = true,

            IsEmailVerified = true,

            PasswordHash =
                passwordHasher.Hash("Admin@123456")
        };

        await dbContext.Users.AddAsync(
            superAdmin,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        Role superAdminRole =
            await dbContext.Roles
                .FirstAsync(
                    r => r.Code == "SUPER_ADMIN",
                    cancellationToken);

        await dbContext.UserRoles.AddAsync(
            new UserRole
            {
                UserId = superAdmin.Id,
                RoleId = superAdminRole.Id
            },
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}
