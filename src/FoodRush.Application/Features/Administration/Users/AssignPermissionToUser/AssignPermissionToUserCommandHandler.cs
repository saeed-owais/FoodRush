using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;

internal sealed class AssignPermissionToUserCommandHandler
    (IApplicationDbContext _dbContext,
    IUserSecurityStampService securityStampService)
    : IRequestHandler<AssignPermissionToUserCommand, Result>
{
    public async Task<Result> Handle(AssignPermissionToUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await _dbContext.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        bool permissionExists = await _dbContext.Permissions
            .AnyAsync(p => p.Id == request.PermissionId, cancellationToken);

        if (!permissionExists)
        {
            return Result.Failure(PermissionErrors.NotFound(request.PermissionId));
        }

        bool alreadyAssigned = await _dbContext.UserPermissions
            .AnyAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId, cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure(PermissionErrors.AlreadyAssignedToUser(request.PermissionId, request.UserId));
        }

        UserPermission userPermission = new()
        {
            UserId = request.UserId,
            PermissionId = request.PermissionId
        };

        await _dbContext.UserPermissions.AddAsync(userPermission, cancellationToken);

        string newSecurityStamp = Guid.NewGuid().ToString();

        await _dbContext.Users
            .Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(
                u => u.SetProperty(
                    user => user.SecurityStamp,
                    newSecurityStamp),
                cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await securityStampService.SetAsync(request.UserId, newSecurityStamp, cancellationToken);

        return Result.Success();
    }
}
