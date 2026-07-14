using Dapper;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Restaurants.Queries;
using FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;
using FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;
using FoodRush.Application.Features.Restaurants.Onboarding;
using FoodRush.Domain.Restaurants.Enums;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Infrastructure.Persistence.Queries;

internal sealed class RestaurantQueries : IRestaurantQueries
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    public RestaurantQueries(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<RestaurantOnboardingResponse?> GetMyRestaurantOnboardingQuery(
        RestaurantId restaurantId,
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
        SELECT
            r.Id,
            r.Name,
            r.Status
        From Restaurants.Restaurants r
        WHERE r.Id = @RestaurantId;

        SELECT
            d.Id,
            d.Type,
            d.Status,
            d.FileUrl,
            d.RejectionReason
        FROM Restaurants.RestaurantDocuments d
        WHERE d.RestaurantId = @RestaurantId
        ORDER BY d.Type
        """;

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                new { RestaurantId = restaurantId.Value });

        var restaurant = await multi.ReadSingleOrDefaultAsync<RestaurantOnboardingResponse>();

        if (restaurant == null)
            return null;

        restaurant.Documents.AddRange(await multi.ReadAsync<RestaurantOnboardingDocumentResponse>());

        return restaurant;
    }

    public async Task<RestaurantOwnerInfo?> GetOwnerInfoAsync(
    RestaurantId restaurantId,
    CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
        SELECT
            u.DisplayName AS Name,
            u.Email AS Email
        FROM Restaurants.Restaurants r
        INNER JOIN [identity].Users u
            ON u.Id = r.OwnerId
        WHERE r.Id = @RestaurantId;
        """;

        CommandDefinition command = new(
            sql,
            new
            {
                RestaurantId = restaurantId.Value
            },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<RestaurantOwnerInfo>(command);
    }

    public async Task<RestaurantDetailsForReviewResponse?> GetRestaurantDetailsForReviewAsync(
        RestaurantId restaurantId,
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
        SELECT
            r.Id,
            r.Name,
            u.DisplayName AS OwnerName,
            u.Email AS OwnerEmail,
            r.Latitude,
            r.Longitude,
            r.DeliveryRadiusKm
        FROM Restaurants.Restaurants r
        INNER JOIN [identity].Users u
            ON u.Id = r.OwnerId
        WHERE r.Id = @RestaurantId
        AND r.Status = @Status;

        SELECT
            d.Id,
            d.Type,
            d.Status,
            d.FileUrl
        FROM Restaurants.RestaurantDocuments d
        WHERE d.RestaurantId = @RestaurantId
        ORDER BY d.Type;
        """;

        using var multi = await connection.QueryMultipleAsync(
        sql,
        new
        {
            RestaurantId = restaurantId.Value,
            Status = RestaurantStatus.UnderReview.ToString()
        });

        var restaurant = await multi.ReadSingleOrDefaultAsync<RestaurantDetailsForReviewResponse>();

        if (restaurant is null)
            return null;

        restaurant.Documents.AddRange(
            await multi.ReadAsync<RestaurantDocumentResponse>());

        return restaurant;
    }

    public async Task<PaginatedResponse<RestaurantDto>> SearchRestaurantsAsync(
       SearchRestaurantsQuery request,
       CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        DynamicParameters parameters = new();

        var whereConditions = new List<string>
        {
            "IsDeleted = 0"
        };

        if (request.Status.HasValue)
        {
            whereConditions.Add("Status = @Status");
            parameters.Add("Status", request.Status.Value.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            whereConditions.Add("Name LIKE @SearchTerm");
            parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
        }

        if (request.OwnerId.HasValue)
        {
            whereConditions.Add("OwnerId = @OwnerId");
            parameters.Add("OwnerId", request.OwnerId.Value);
        }

        if (request.CreatedAfter.HasValue)
        {
            whereConditions.Add("CreatedAt >= @CreatedAfter");
            parameters.Add("CreatedAfter", request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            whereConditions.Add("CreatedAt <= @CreatedBefore");
            parameters.Add("CreatedBefore", request.CreatedBefore.Value);
        }

        string whereClause = string.Join(" AND ", whereConditions);

        string orderBy = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => "Name",
            "status" => "Status",
            "submittedat" => "CreatedAt",
            _ => "CreatedAt"
        };

        string sortDirection =
            request.IsDescending ? "DESC" : "ASC";

        int offset =
            (request.PageNumber - 1) * request.PageSize;

        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        string sql = $"""
        SELECT
            {nameof(RestaurantDto.Id)},
            {nameof(RestaurantDto.Name)},
            {nameof(RestaurantDto.Status)},
            CreatedAt AS SubmittedAt
        FROM Restaurants.Restaurants
        WHERE {whereClause}
        ORDER BY {orderBy} {sortDirection}
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Restaurants.Restaurants
        WHERE {whereClause};
        """;

        CommandDefinition command = new(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var restaurants =
            (await multi.ReadAsync<RestaurantDto>())
            .ToList();

        int totalCount =
        await multi.ReadSingleAsync<int>();

        return new PaginatedResponse<RestaurantDto>(
            restaurants,
            request.PageNumber,
            request.PageSize,
            totalCount);
    }
}
