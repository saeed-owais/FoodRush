using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodRush.Infrastructure.Persistence.Configurations.Restaurants;

internal sealed class RestaurantConfiguration
: IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("Restaurants", "Restaurants");

        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.OwnerId);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new RestaurantId(value));

        builder.Property(r => r.OwnerId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.OwnsOne(r => r.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(r => r.AverageRating, rating =>
        {
            rating.Property(r => r.Value)
                .HasColumnName("AverageRating");
        });

        builder.OwnsOne(r => r.Latitude, latitude =>
        {
            latitude.Property(l => l.Value)
                .HasColumnName("Latitude");
        });

        builder.OwnsOne(r => r.Longitude, longitude =>
        {
            longitude.Property(l => l.Value)
                .HasColumnName("Longitude");
        });

        builder.OwnsOne(r => r.DeliveryRadiusKm, radius =>
        {
            radius.Property(r => r.Value)
                .HasColumnName("DeliveryRadiusKm");
        });

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Ignore(r => r.IsVisible);

        builder.HasMany(r => r.Documents)
            .WithOne()
            .HasForeignKey(d => d.RestaurantId);

        builder.Navigation(r => r.Documents)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}