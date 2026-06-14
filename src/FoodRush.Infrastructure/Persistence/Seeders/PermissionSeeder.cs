using FoodRush.Application.Common.Authorization;
using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Seeders;

internal static class PermissionSeeder
{
    public static async Task SeedPermissionsAsync(
       ApplicationDbContext dbContext,
       CancellationToken cancellationToken = default)
    {
        IEnumerable<(string Name, string Code)> permissions =
            GetPermissions();

        foreach ((string name, string code) in permissions)
        {
            Permission? existingPermission =
                await dbContext.Permissions
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(
                        p => p.Code == code,
                        cancellationToken);

            if (existingPermission is null)
            {
                await dbContext.Permissions.AddAsync(
                    new Permission
                    {
                        Name = name,
                        Code = code
                    },
                    cancellationToken);

                continue;
            }

            if (existingPermission.IsDeleted)
            {
                existingPermission.IsDeleted = false;
                existingPermission.DeletedAt = null;
                existingPermission.DeletedBy = null;
                existingPermission.Name = name;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<(string Name, string Code)> GetPermissions()
    {
        Type permissionsType = typeof(Permissions);

        foreach (Type nestedType in permissionsType.GetNestedTypes())
        {
            string groupName = nestedType.Name;

            foreach (var field in nestedType.GetFields(
                         System.Reflection.BindingFlags.Public |
                         System.Reflection.BindingFlags.Static |
                         System.Reflection.BindingFlags.FlattenHierarchy))
            {
                if (!field.IsLiteral ||
                    field.IsInitOnly ||
                    field.FieldType != typeof(string))
                {
                    continue;
                }

                string code =
                    (string)field.GetRawConstantValue()!;

                string name =
                    $"{groupName} {field.Name}";

                yield return (name, code);
            }
        }
    }
}

