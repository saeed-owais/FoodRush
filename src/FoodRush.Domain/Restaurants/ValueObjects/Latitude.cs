using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.ValueObjects;

public sealed record Latitude
{
    public double Value { get; }
    private Latitude(double value)
    {
        Value = value;
    }
    public static Result<Latitude> Create(double latitude)
    {
        if (latitude > 90 || latitude < -90)
        {
            return Result.Failure<Latitude>(RestaurantErrors.InvalidLatitude);
        }
        return Result.Success(new Latitude(latitude));
    }
}