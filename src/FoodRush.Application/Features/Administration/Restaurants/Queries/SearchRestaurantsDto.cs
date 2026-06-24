using FoodRush.Domain.Restaurants.Enums;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries;

public sealed record SearchRestaurantsDto(
    RestaurantStatus? Status,
    string? SearchTerm,
    Guid? OwnerId,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    string? SortBy,
    bool IsDescending,
    int PageNumber,
    int PageSize)
    ;