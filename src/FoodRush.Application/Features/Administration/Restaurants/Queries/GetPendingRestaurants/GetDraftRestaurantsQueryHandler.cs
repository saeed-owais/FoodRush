using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Common.Models;
using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Enums;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetPendingRestaurants;

internal sealed class GetDraftRestaurantsQueryHandler
    (IRestaurantQueries restaurantQueries)
    : IQueryHandler<GetDraftRestaurantsQuery, PaginatedResponse<RestaurantDto>>
{
    public async Task<Result<PaginatedResponse<RestaurantDto>>> Handle(GetDraftRestaurantsQuery request, CancellationToken cancellationToken)
    {
        SearchRestaurantsDto searchRestaurantsDto = new
        (
            Status: RestaurantStatus.Draft,
            SearchTerm: request.SearchTerm,
            OwnerId: request.OwnerId,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            CreatedAfter: request.CreatedAfter,
            CreatedBefore: request.CreatedBefore,
            IsDescending: request.IsDescending,
            SortBy: request.SortBy
        );

        var result = await restaurantQueries.SearchRestaurantsAsync(searchRestaurantsDto, cancellationToken);

        return Result.Success<PaginatedResponse<RestaurantDto>>(result);
    }
}
