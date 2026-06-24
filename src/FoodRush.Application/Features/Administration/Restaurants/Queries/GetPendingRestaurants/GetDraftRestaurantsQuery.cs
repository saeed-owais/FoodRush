using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Common.Models;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetPendingRestaurants;

public sealed record GetDraftRestaurantsQuery(
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
