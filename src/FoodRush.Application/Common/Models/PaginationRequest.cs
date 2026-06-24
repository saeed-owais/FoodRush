namespace FoodRush.Application.Common.Models;

public record PaginationRequest
{
    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public PaginationRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber <= 0 ? 1 : pageNumber;
        PageSize = pageSize <= 0 ? 10 : pageSize;
    }
}