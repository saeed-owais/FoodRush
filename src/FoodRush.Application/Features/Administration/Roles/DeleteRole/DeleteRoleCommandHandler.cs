using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.DeleteRole;

internal sealed class DeleteRoleCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await _dbContext.Roles
            .AsTracking()
            .FirstOrDefaultAsync(
                r => r.Id == request.Id,
                cancellationToken);

        if (role == null)
        {
            return Result.Failure(RoleErrors.NotFound(request.Id));
        }

        _dbContext.Roles.Remove(role);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

