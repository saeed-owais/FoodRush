using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Users.AssignRoleToUser;

internal sealed class AssignRoleToUserHandler
    (IApplicationDbContext dbContext,
    IUserSecurityStampService securityStampService,
    ILogger<AssignRoleToUserHandler> logger)
    : IRequestHandler<AssignRoleToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        bool userExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        bool roleExists = await dbContext.Roles
            .AnyAsync(r => r.Id == request.RoleId, cancellationToken);

        if (!roleExists)
        {
            return Result.Failure(RoleErrors.NotFound(request.RoleId));
        }

        bool isRoleAssignedToUser = await dbContext.UserRoles
            .AnyAsync(ur =>
                ur.UserId == request.UserId &&
                ur.RoleId == request.RoleId,
            cancellationToken);

        if (isRoleAssignedToUser)
        {
            return Result.Failure(
                RoleErrors.AlreadyAssignedToUser(request.RoleId, request.UserId));
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        string securityStamp = Guid.NewGuid().ToString();
        try
        {
            await dbContext.UserRoles
                .AddAsync(new UserRole
                {
                    UserId = request.UserId,
                    RoleId = request.RoleId,
                }, cancellationToken);

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
                "An error occurred while assigning role {RoleId} to user {UserId}",
                request.RoleId,
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
                "Failed to update security stamp for user {UserId}",
                request.UserId);
        }

        return Result.Success();
    }
}
