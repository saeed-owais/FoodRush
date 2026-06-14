using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.GetRolePermissions;

public sealed record GetRolePermissionsQuery(Guid RoleId)
    : IRequest<Result<List<RolePermissionResponse>>>;