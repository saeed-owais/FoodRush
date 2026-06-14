using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;

internal sealed class RemovePermissionFromUserCommandHandler
    (IApplicationDbContext dbContext,
    IUserSecurityStampService securityStampService,
    ILogger<RemovePermissionFromUserCommandHandler> logger)
    : IRequestHandler<RemovePermissionFromUserCommand, Result>
{
    public async Task<Result> Handle(RemovePermissionFromUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await dbContext.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        bool permissionExists = await dbContext.Permissions
            .AnyAsync(p => p.Id == request.PermissionId, cancellationToken);

        if (!permissionExists)
        {
            return Result.Failure(PermissionErrors.NotFound(request.PermissionId));
        }

        UserPermission? userPermission = await dbContext.UserPermissions
            .AsTracking()
            .FirstOrDefaultAsync(
                up => up.UserId == request.UserId &&
                    up.PermissionId == request.PermissionId,
                cancellationToken);

        if (userPermission == null)
        {
            return Result.Failure(Error.NotFound("UserPermission.NotFound", $"Permission with ID {request.PermissionId} is not assigned to user with ID {request.UserId}."));
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        string securityStamp = Guid.NewGuid().ToString();

        try
        {
            dbContext.UserPermissions.Remove(userPermission);

            await dbContext.Users
                .Where(u => u.Id == request.UserId)
                .ExecuteUpdateAsync(
                    u => u.SetProperty(
                        user => user.SecurityStamp,
                        securityStamp),
                    cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while removing permission {PermissionId} from user {UserId}.",
                request.PermissionId,
                request.UserId);

            throw;
        }

        try
        {
            await securityStampService.SetAsync(request.UserId, securityStamp, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to update security stamp for user {UserId} after removing permission {PermissionId}.",
                request.UserId,
                request.PermissionId);
        }

        return Result.Success();
    }
}
