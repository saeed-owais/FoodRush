using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Common.Models;
using FoodRush.Domain.Restaurants.Enums;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;

public sealed record SearchRestaurantsQuery(
    RestaurantStatus? Status,
    string? SearchTerm,
    Guid? OwnerId,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    string? SortBy,
    bool IsDescending,
    int PageNumber,
    int PageSize)
    : PaginationRequest(PageNumber, PageSize),
    IQuery<PaginatedResponse<RestaurantDto>>;
