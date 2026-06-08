namespace FoodRush.Application.Common.Models;

public record PaginationRequest(
    int PageNumber = 1,
    int PageSize = 10);