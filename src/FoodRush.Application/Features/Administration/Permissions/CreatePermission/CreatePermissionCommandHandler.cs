using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Permissions.CreatePermission;

internal sealed class CreatePermissionCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<CreatePermissionCommand, Result<CreatePermissionResponse>>
{
    public async Task<Result<CreatePermissionResponse>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        Permission? permission = await _dbContext.Permissions
            .IgnoreQueryFilters()
            .AsTracking()
            .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

        if (permission != null)
        {
            if (!permission.IsDeleted)
            {
                return Result.Failure<CreatePermissionResponse>(PermissionErrors.AlreadyExists(request.Code));
            }

            permission.IsDeleted = false;
            permission.DeletedAt = null;
            permission.DeletedBy = null;

            permission.Name = request.Name;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreatePermissionResponse(
                permission.Id,
                permission.Name,
                permission.Code);
        }

        Permission newPermission = new Permission
        {
            Name = request.Name,
            Code = request.Code,
        };

        await _dbContext.Permissions.AddAsync(newPermission, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreatePermissionResponse(
            newPermission.Id,
            newPermission.Name,
            newPermission.Code);
    }
}
