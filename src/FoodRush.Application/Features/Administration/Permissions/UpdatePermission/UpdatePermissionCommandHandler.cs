using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Permissions.UpdatePermission;

internal sealed class UpdatePermissionCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<UpdatePermissionCommand, Result<UpdatePermissionResponse>>
{
    public async Task<Result<UpdatePermissionResponse>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        Permission? permission = await _dbContext.Permissions
            .AsTracking()
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (permission == null)
        {
            return Result.Failure<UpdatePermissionResponse>(PermissionErrors.NotFound(request.Id));
        }

        permission.Name = request.Name;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdatePermissionResponse(
            permission.Id,
            permission.Name,
            permission.Code);
    }
}
