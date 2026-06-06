using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Infrastructure.Persistence;
using FoodRush.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.API.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();

            IPasswordHasher passwordHasher =
                scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await IdentitySeeder.SeedAsync(dbContext, passwordHasher);
        }
    }
}
