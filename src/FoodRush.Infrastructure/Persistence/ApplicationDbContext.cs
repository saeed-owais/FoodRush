using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodRush.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IUserContext _userContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IUserContext userContext)
        : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<UserPermission> UserPermissions { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<RevokedToken> RevokedTokens { get; set; }

    public DbSet<OtpRequest> OtpRequests { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        DateTime utcNow = DateTime.UtcNow;

        Guid? currentUserId =
            _userContext.IsAuthenticated
                ? _userContext.UserId
                : null;

        foreach (var entry in ChangeTracker
            .Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedAt = utcNow;

                    entry.Entity.CreatedBy = currentUserId;

                    entry.Entity.UpdatedAt = utcNow;

                    entry.Entity.UpdatedBy = currentUserId;

                    break;

                case EntityState.Modified:

                    entry.Entity.UpdatedAt = utcNow;

                    entry.Entity.UpdatedBy = currentUserId;

                    break;

                case EntityState.Deleted:

                    entry.State = EntityState.Modified;

                    entry.Entity.IsDeleted = true;

                    entry.Entity.DeletedAt = utcNow;

                    entry.Entity.DeletedBy = currentUserId;

                    entry.Entity.UpdatedAt = utcNow;

                    entry.Entity.UpdatedBy = currentUserId;

                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteQueryFilters(
        ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model
            .GetEntityTypes())
        {
            if (typeof(ISoftDeletable)
                .IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(
                        GenerateIsDeletedFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression GenerateIsDeletedFilter(
        Type entityType)
    {
        var parameter =
            Expression.Parameter(entityType, "e");

        var property =
            Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));

        var body =
            Expression.Equal(property, Expression.Constant(false));

        return Expression.Lambda(body, parameter);
    }
}
