using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.RemovePermissionFromRole;

public sealed record RemovePermissionFromRoleCommand(Guid RoleId, Guid PermissionId)
    : IRequest<Result>;
