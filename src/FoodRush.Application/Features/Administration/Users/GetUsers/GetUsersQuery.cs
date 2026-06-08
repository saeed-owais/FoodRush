using FoodRush.Application.Common;
using FoodRush.Application.Common.Models;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.GetUsers;

public sealed record GetUsersQuery(
    string? SearchTerm,
    string? Email,
    string? PhoneNumber,
    bool? IsActive,
    bool? IsEmailVerified,
    Guid? RoleId,
    string SortBy = "createdat",
    bool IsDescending = true,
    int PageNumber = 1,
    int PageSize = 10)
    : PaginationRequest(PageNumber, PageSize),
      IRequest<Result<PaginatedResponse<GetUsersResponse>>>;
