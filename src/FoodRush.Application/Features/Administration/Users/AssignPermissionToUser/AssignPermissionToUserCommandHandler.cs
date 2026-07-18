using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;

internal sealed class AssignPermissionToUserCommandHandler
    (IApplicationDbContext _dbContext,
    IUserSecurityStampService securityStampService,
    ILogger<AssignPermissionToUserCommandHandler> logger)
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

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        string newSecurityStamp = Guid.NewGuid().ToString();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                await _dbContext.UserPermissions.AddAsync(
                    userPermission,
                    cancellationToken);

                await _dbContext.Users
                    .Where(u => u.Id == request.UserId)
                    .ExecuteUpdateAsync(
                        u => u.SetProperty(
                            user => user.SecurityStamp,
                            newSecurityStamp),
                        cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to assign permission {PermissionId} to user {UserId}",
                request.PermissionId,
                request.UserId);

            throw;
        }

        try
        {
            await securityStampService.SetAsync(
                request.UserId,
                newSecurityStamp,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to update security stamp cache for user {UserId}",
                request.UserId);
        }

        return Result.Success();
    }
}
