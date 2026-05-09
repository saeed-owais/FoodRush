using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Identity
{
    public sealed class RevokedTokenConfiguration
    : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(EntityTypeBuilder<RevokedToken> builder)
        {
            builder.ToTable("RevokedTokens", "identity");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.JwtId)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.JwtId)
                .IsUnique();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.RevokedAt)
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
