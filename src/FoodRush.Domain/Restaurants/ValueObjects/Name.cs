using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.ValueObjects;

public sealed record Name
{
    private readonly string _value;
    public string Value => _value;
    private Name(string value)
    {
        _value = value;
    }

    public static Result<Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Name>(RestaurantErrors.NameIsRequired);
        }

        name = name.Trim();

        if (name.Length > 100)
        {
            return Result.Failure<Name>(RestaurantErrors.NameTooLong);
        }
        return Result.Success(new Name(name));
    }
}