using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.ValueObjects;

public sealed record DeliveryRadiusKm
{
    private readonly double _value;
    public double Value => _value;
    private DeliveryRadiusKm(double value)
    {
        _value = value;
    }
    public static Result<DeliveryRadiusKm> Create(double deliveryRadiusKm = 50)
    {
        if (deliveryRadiusKm < 0)
        {
            return Result.Failure<DeliveryRadiusKm>(RestaurantErrors.DeliveryRadiusCannotBeNegative);
        }
        return Result.Success(new DeliveryRadiusKm(deliveryRadiusKm));
    }
}