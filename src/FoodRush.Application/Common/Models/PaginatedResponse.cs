namespace FoodRush.Application.Common.Models;

public sealed record PaginatedResponse<T>(
    IReadOnlyCollection<T> Data,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public bool HasNextPage => PageNumber < TotalPages;

    public bool HasPreviousPage => PageNumber > 1;
}