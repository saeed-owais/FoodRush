using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Identity
{
    public sealed class UserConfiguration
        : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "identity");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(x => x.NormalizedEmail)
                .IsUnique();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            builder.HasIndex(x => x.PhoneNumber)
                .IsUnique();

            builder.Property(x => x.NormalizedPhoneNumber)
                .HasMaxLength(20);

            builder.HasIndex(x => x.NormalizedPhoneNumber)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.AvatarUrl)
                .HasMaxLength(512);

            builder.Property(x => x.IsEmailVerified)
                .HasDefaultValue(false);

            builder.Property(x => x.IsPhoneVerified)
                .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.SecurityStamp)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.FcmToken)
                .HasMaxLength(512);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
