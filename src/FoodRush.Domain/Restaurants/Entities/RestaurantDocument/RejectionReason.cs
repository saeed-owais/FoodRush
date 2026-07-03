using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public sealed record RejectionReason
{
    private readonly string _value;
    public string Value => _value;
    private RejectionReason(string value)
    {
        _value = value;
    }
    public static Result<RejectionReason> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<RejectionReason>(RestaurantErrors.InvalidDocumentRejectionReason);
        }
        return Result.Success(new RejectionReason(value));
    }
}