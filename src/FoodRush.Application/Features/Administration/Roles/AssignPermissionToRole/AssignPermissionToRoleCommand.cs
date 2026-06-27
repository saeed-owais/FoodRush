using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.AssignPermissionToRole;

public sealed record AssignPermissionToRoleCommand(Guid RoleId, Guid PermissionId)
    : IRequest<Result>;
