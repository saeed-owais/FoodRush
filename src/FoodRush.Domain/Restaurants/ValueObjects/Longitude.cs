using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.ValueObjects;

public sealed record Longitude
{
    private readonly double _value;
    public double Value => _value;
    private Longitude(double value)
    {
        _value = value;
    }
    public static Result<Longitude> Create(double longitude)
    {
        if (longitude < -180 || longitude > 180)
        {
            return Result.Failure<Longitude>(RestaurantErrors.InvalidLongitude);
        }
        return Result.Success(new Longitude(longitude));
    }
}