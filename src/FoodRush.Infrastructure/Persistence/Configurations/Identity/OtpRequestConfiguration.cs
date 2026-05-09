using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Identity
{
    public sealed class OtpRequestConfiguration
    : IEntityTypeConfiguration<OtpRequest>
    {
        public void Configure(EntityTypeBuilder<OtpRequest> builder)
        {
            builder.ToTable("OtpRequests", "identity");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.CodeHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.AttemptCount)
                .HasDefaultValue(0);

            builder.Property(x => x.ResendCount)
                .HasDefaultValue(0);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
