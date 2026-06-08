using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Application.Features.Administration.Permissions.GetPermissions;
using FoodRush.Application.Features.Administration.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler
    (IApplicationDbContext dbContext)
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        GetUserByIdResponse? user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new GetUserByIdResponse(
                u.Id,
                u.DisplayName,
                u.Email,
                u.PhoneNumber,
                u.AvatarUrl,
                u.IsActive,
                u.IsEmailVerified,
                u.IsPhoneVerified,
                u.FailedLoginAttempts,
                u.LockoutEnd,
                u.LastLoginAt,
                u.CreatedAt,
                u.UpdatedAt,

                Roles: u.UserRoles
                    .Select(ur => new RoleResponse(
                        ur.Role.Id,
                        ur.Role.Name,
                        ur.Role.Code))
                    .ToList(),

                DirectPermissions: u.UserPermissions
                    .Select(up => new PermissionResponse(
                        up.Permission.Id,
                        up.Permission.Name,
                        up.Permission.Code))
                    .ToList(),

                RolePermissions: u.UserRoles
                    .SelectMany(ur =>
                        ur.Role.RolePermissions
                        .Select(rp => new PermissionResponse(
                            rp.Permission.Id,
                            rp.Permission.Name,
                            rp.Permission.Code)))
                    .Distinct()
                    .ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result.Failure<GetUserByIdResponse>(Error.NotFound("User.NotFound", $"User with ID {request.UserId} not found."));
        }

        return Result.Success(user);
    }
}