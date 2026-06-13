using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.GetUserPrfile;

internal sealed class GetUserPrfileQueryHandler
(
    IApplicationDbContext dbContext,
    IUserContext userContext
) : IRequestHandler<GetUserPrfileQuery, Result<GetUserProfileResponse>>
{
    public async Task<Result<GetUserProfileResponse>> Handle(GetUserPrfileQuery request, CancellationToken cancellationToken)
    {
        var userData = await dbContext.Users
            .AsNoTracking()
            .Select(u => new
            {

                Id = u.Id,
                DisplayName = u.DisplayName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                IsEmailVerified = u.IsEmailVerified,
                IsPhoneVerified = u.IsPhoneVerified,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Code).ToList(),
                RolePermissions =
                    u.UserRoles
                        .SelectMany(ur => ur.Role.RolePermissions)
                        .Select(rp => rp.Permission.Code)
                        .Distinct()
                        .ToList(),
                UserPermissions = u.UserPermissions.Select(up => up.Permission.Code)
            }
            )
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (userData is null)
        {
            return Result.Failure<GetUserProfileResponse>(
               UserErrors.NotFound(userContext.UserId)
                );
        }

        List<string> permissions = userData.RolePermissions.Union(userData.UserPermissions).ToList();

        GetUserProfileResponse user = new(
            userData.Id,
            userData.DisplayName,
            userData.Email,
            userData.PhoneNumber,
            userData.AvatarUrl,
            userData.IsEmailVerified,
            userData.IsPhoneVerified,
            userData.IsActive,
            userData.CreatedAt,
            userData.Roles,
            permissions
            );

        return user;
    }
}
