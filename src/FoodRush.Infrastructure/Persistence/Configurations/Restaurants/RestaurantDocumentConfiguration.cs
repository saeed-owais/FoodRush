using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Restaurants;

internal sealed class RestaurantDocumentConfiguration
    : IEntityTypeConfiguration<RestaurantDocument>
{
    public void Configure(EntityTypeBuilder<RestaurantDocument> builder)
    {
        builder.ToTable("RestaurantDocuments", "Restaurants");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new DocumentId(value));

        builder.Property(d => d.RestaurantId)
            .HasConversion(
                id => id.Value,
                value => new RestaurantId(value));

        builder.OwnsOne(d => d.PublicId, publicId =>
        {
            publicId.Property(p => p.Value)
                .HasColumnName("PublicId")
                .HasMaxLength(255)
                .IsRequired();
        });

        builder.OwnsOne(d => d.FileUrl, fileUrl =>
        {
            fileUrl.Property(f => f.Url)
                .HasColumnName("FileUrl")
                .HasMaxLength(2048)
                .IsRequired();
        });

        builder.Property(d => d.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}