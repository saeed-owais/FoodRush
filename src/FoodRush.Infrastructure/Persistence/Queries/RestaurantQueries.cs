using Dapper;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;

namespace FoodRush.Infrastructure.Persistence.Queries;

internal sealed class RestaurantQueries : IRestaurantQueries
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    public RestaurantQueries(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
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
