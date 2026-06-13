using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FoodRush.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; }

        public DbSet<Role> Roles { get; }

        public DbSet<Permission> Permissions { get; }

        public DbSet<UserRole> UserRoles { get; }

        public DbSet<RolePermission> RolePermissions { get; }

        public DbSet<UserPermission> UserPermissions { get; }

        public DbSet<RefreshToken> RefreshTokens { get; }

        public DbSet<OtpRequest> OtpRequests { get; }

        public DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
