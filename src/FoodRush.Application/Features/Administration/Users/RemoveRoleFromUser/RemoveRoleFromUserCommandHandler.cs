using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Users.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandHandler
    (IApplicationDbContext dbContext,
    IUserSecurityStampService securityStampService,
    ILogger<RemoveRoleFromUserCommandHandler> logger)
    : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        UserRole? userRole = await dbContext.UserRoles
            //.AsTracking()
            .FirstOrDefaultAsync(ur =>
                ur.UserId == request.UserId &&
                ur.RoleId == request.RoleId,
            cancellationToken);

        if (userRole == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "UserRole.NotFound",
                    $"The specified role with ID {request.RoleId} is not assigned to the user with ID {request.UserId}.")
            );
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        string securityStamp = Guid.NewGuid().ToString();

        try
        {
            await dbContext.Users
            .Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(
                u => u.SetProperty(
                    user => user.SecurityStamp,
                    securityStamp),
                cancellationToken);

            dbContext.UserRoles.Remove(userRole);
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while removing role with ID {RoleId} from user with ID {UserId}.",
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
                "Failed to update security stamp for user {UserId} after removing role {RoleId}.",
                request.UserId,
                request.RoleId);
        }

        return Result.Success();
    }
}
