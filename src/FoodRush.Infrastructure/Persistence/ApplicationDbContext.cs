using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Interfaces;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodRush.Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region Properties
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<OtpRequest> OtpRequests { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<RestaurantDocument> RestaurantDocuments { get; set; }
    #endregion

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddInboxStateEntity();

        ApplySoftDeleteQueryFilters(modelBuilder);
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
