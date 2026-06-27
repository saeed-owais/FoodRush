using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public sealed record PublicId
{
    private readonly string _value;
    public string Value => _value;
    private PublicId(string value)
    {
        _value = value;
    }
    public static Result<PublicId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PublicId>(RestaurantErrors.InvalidDocumentPublicId);
        }
        return Result.Success(new PublicId(value));
    }

}