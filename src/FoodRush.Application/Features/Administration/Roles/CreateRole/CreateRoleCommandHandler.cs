using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Roles.CreateRole;

internal sealed class CreateRoleCommandHandler
    (IApplicationDbContext _dbContext)
    : IRequestHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    public async Task<Result<CreateRoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        bool roleExists = await _dbContext.Roles.AnyAsync(r => r.Code == request.Code, cancellationToken);

        if (roleExists)
        {
            return Result.Failure<CreateRoleResponse>(
                Error.Conflict(
                    "Role.AlreadyExists",
                    $"Role with code '{request.Code}' already exists."));
        }

        Role newRole = new Role
        {
            Name = request.Name,
            Code = request.Code,
        };

        await _dbContext.Roles.AddAsync(newRole, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateRoleResponse(
            newRole.Id,
            newRole.Name,
            newRole.Code);
    }
}
