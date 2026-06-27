using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;

namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public sealed record FileUrl
{
    private readonly string _url;
    public string Url => _url;
    private FileUrl(string url)
    {
        _url = url;
    }

    public static Result<FileUrl> Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure<FileUrl>(RestaurantErrors.InvalidDocumentUrl);
        }
        return Result.Success(new FileUrl(url));
    }
}