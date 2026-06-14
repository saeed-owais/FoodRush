using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.UpdateRole;

internal sealed class UpdateRoleCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<UpdateRoleCommand, Result<UpdateRoleResponse>>
{
    public async Task<Result<UpdateRoleResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        Role? role = await _dbContext.Roles
            .AsTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
        {
            return Result.Failure<UpdateRoleResponse>(RoleErrors.NotFound(request.Id));
        }

        bool codeExists = await _dbContext.Roles
            .AnyAsync(
            r => r.Id != request.Id &&
                r.Code == request.Code,
            cancellationToken);

        if (codeExists)
        {
            return Result.Failure<UpdateRoleResponse>(RoleErrors.AlreadyExists(request.Code));
        }

        role.Name = request.Name;
        role.Code = request.Code;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateRoleResponse(
            role.Id,
            role.Name,
            role.Code);

    }
}

