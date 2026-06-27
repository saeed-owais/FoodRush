using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RolesConstants = FoodRush.Application.Common.Authorization.Roles;

namespace FoodRush.Application.Features.Administration.Users.DeleteUser;

internal sealed class DeleteUserCommandHnadler
    (IApplicationDbContext dbContext)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId,
                cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        bool isSuperAdmin = await dbContext.UserRoles
            .AnyAsync(
                ur =>
                    ur.UserId == request.UserId &&
                    ur.Role.Code == RolesConstants.SuperAdmin,
                cancellationToken);

        if (isSuperAdmin)
        {
            int superAdminsCount = await dbContext.UserRoles
                .Where(ur => ur.Role.Code == RolesConstants.SuperAdmin)
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            if (superAdminsCount <= 1)
            {
                return Result.Failure(UserErrors.LastSuperAdmin);
            }
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
