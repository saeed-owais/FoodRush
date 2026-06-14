using FoodRush.Application.Common.Authorization;
using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        IEnumerable<(string Name, string Code)> roles =
            GetRoles();

        foreach ((string Name, string Code) role in roles)
        {
            Role? existingRole =
                await dbContext.Roles
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        r => r.Code == role.Code,
                        cancellationToken);

            if (existingRole is null)
            {
                await dbContext.Roles.AddAsync(
                    new Role
                    {
                        Name = role.Name,
                        Code = role.Code
                    },
                    cancellationToken);

                continue;
            }

            if (existingRole.IsDeleted)
            {
                existingRole.IsDeleted = false;
                existingRole.DeletedAt = null;
                existingRole.DeletedBy = null;
            }

            existingRole.Name = role.Name;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<(string Name, string Code)> GetRoles()
    {
        FieldInfo[] fields =
            typeof(Roles)
                .GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.FlattenHierarchy);

        foreach (FieldInfo field in fields)
        {
            if (!field.IsLiteral ||
                field.IsInitOnly ||
                field.FieldType != typeof(string))
            {
                continue;
            }

            string code =
                (string)field.GetRawConstantValue()!;

            yield return (
                Name: field.Name,
                Code: code);
        }
    }
}