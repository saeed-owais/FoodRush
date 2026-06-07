using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
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
        bool roleExists = await _dbContext.Permissions
            .AnyAsync(r => r.Code == request.Code, cancellationToken);

        if (roleExists)
        {
            return Result.Failure<CreatePermissionResponse>(
                Error.Conflict(
                    "Permission.AlreadyExists",
                    $"Permission with code '{request.Code}' already exists."));
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
