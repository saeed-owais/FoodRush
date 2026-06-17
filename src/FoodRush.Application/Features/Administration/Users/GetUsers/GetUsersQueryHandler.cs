using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common.Models;
using FoodRush.Domain.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.GetUsers;

internal sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<PaginatedResponse<GetUsersResponse>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetUsersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PaginatedResponse<GetUsersResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<User> query = _dbContext.Users
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string searchTerm = request.SearchTerm.Trim();

            query = query.Where(u =>
                EF.Functions.Like(
                    u.DisplayName,
                    $"%{searchTerm}%") ||

                EF.Functions.Like(
                    u.Email,
                    $"%{searchTerm}%") ||

                (u.PhoneNumber != null &&
                 EF.Functions.Like(
                     u.PhoneNumber,
                     $"%{searchTerm}%")));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            string email = request.Email.Trim();

            query = query.Where(u =>
                u.Email == email);
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            string phoneNumber = request.PhoneNumber.Trim();

            query = query.Where(u =>
                u.PhoneNumber == phoneNumber);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u =>
                u.IsActive == request.IsActive.Value);
        }

        if (request.IsEmailVerified.HasValue)
        {
            query = query.Where(u =>
                u.IsEmailVerified == request.IsEmailVerified.Value);
        }

        if (request.RoleId.HasValue)
        {
            query = query.Where(u =>
                u.UserRoles.Any(
                    ur => ur.RoleId == request.RoleId.Value));
        }

        int totalCount =
            await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request);

        List<GetUsersResponse> users =
            await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new GetUsersResponse(
                    u.Id,
                    u.DisplayName,
                    u.Email,
                    u.PhoneNumber,
                    u.IsActive,
                    u.IsEmailVerified,
                    u.UserRoles.Count,
                    u.CreatedAt,
                    u.LastLoginAt))
                .ToListAsync(cancellationToken);

        return Result.Success(
            new PaginatedResponse<GetUsersResponse>(
                users,
                request.PageNumber,
                request.PageSize,
                totalCount));
    }

    private static IQueryable<User> ApplySorting(
        IQueryable<User> query,
        GetUsersQuery request)
    {
        string sortBy =
            request.SortBy?.Trim().ToLowerInvariant()!;

        return sortBy switch
        {
            "displayname" => request.IsDescending
                ? query.OrderByDescending(u => u.DisplayName)
                : query.OrderBy(u => u.DisplayName),

            "email" => request.IsDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            "lastloginat" => request.IsDescending
                ? query.OrderByDescending(u => u.LastLoginAt)
                : query.OrderBy(u => u.LastLoginAt),

            "createdat" => request.IsDescending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),

            _ => query.OrderByDescending(u => u.CreatedAt)
        };
    }
}