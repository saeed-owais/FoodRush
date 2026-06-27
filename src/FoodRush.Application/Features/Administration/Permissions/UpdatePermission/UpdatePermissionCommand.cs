using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Permissions.UpdatePermission;

public sealed record UpdatePermissionCommand(Guid Id, string Name)
    : IRequest<Result<UpdatePermissionResponse>>;
