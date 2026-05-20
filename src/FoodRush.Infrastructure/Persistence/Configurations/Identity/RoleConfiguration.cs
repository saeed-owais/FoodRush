using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Identity
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public static readonly Guid AdminRoleId =
           Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static readonly Guid CustomerRoleId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid DriverRoleId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid RestaurantOwnerRoleId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "identity");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Code)
                   .HasMaxLength(100);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();



            builder.HasData(
                new Role
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    Code = "ADMIN"
                },
                new Role
                {
                    Id = CustomerRoleId,
                    Name = "Customer",
                    Code = "CUSTOMER"
                },
                new Role
                {
                    Id = DriverRoleId,
                    Name = "Driver",
                    Code = "DRIVER"
                },
                new Role
                {
                    Id = RestaurantOwnerRoleId,
                    Name = "Restaurant Owner",
                    Code = "RESTAURANT_OWNER"
                });
        }
    }
}
